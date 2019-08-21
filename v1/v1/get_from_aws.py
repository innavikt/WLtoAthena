import sys
import pandas as pd
import os
import csv
import boto3
import botocore
from retrying import retry

cred_file = pd.read_csv('credentials.csv')
access_key = cred_file['Access key ID'][0]
secret_key = cred_file['Secret access key'][0]

# configuration
s3_bucket = 'myiqfeed'       # S3 Bucket name
s3_ouput  = 's3://'+ s3_bucket   # S3 Bucket to store results
database  = 'iqfeed'  # The database to which the query belongs

# init clients
session = boto3.Session(
    aws_access_key_id=access_key,
    aws_secret_access_key=secret_key,
)

s3     = session.resource('s3')
athena = session.client('athena', region_name='us-east-1')

@retry(stop_max_attempt_number = 10,
    wait_exponential_multiplier = 300,
    wait_exponential_max = 1 * 60 * 1000)

def poll_status(_id):
    result = athena.get_query_execution( QueryExecutionId = _id )
    state  = result['QueryExecution']['Status']['State']

    if state == 'SUCCEEDED':
        return result
    elif state == 'FAILED':
        return result
    else:
        raise Exception

def run_query(query, database, s3_output):
    response = athena.start_query_execution(
        QueryString=query,
        QueryExecutionContext={
            'Database': database
        },
        ResultConfiguration={
            'OutputLocation': s3_output,
    })

    QueryExecutionId = response['QueryExecutionId']
    result = poll_status(QueryExecutionId)

    if result['QueryExecution']['Status']['State'] == 'SUCCEEDED':
        print("Query SUCCEEDED: {}".format(QueryExecutionId))

        s3_key = QueryExecutionId + '.csv'
        local_filename = QueryExecutionId + '.csv'

        # download result file
        try:
            s3.Bucket(s3_bucket).download_file(s3_key, local_filename)
        except botocore.exceptions.ClientError as e:
            if e.response['Error']['Code'] == "404":
                print("The object does not exist.")
            else:
                raise

        # read file to array
        rows = []
        with open(local_filename) as csvfile:
            reader = csv.DictReader(csvfile)
            for row in reader:
                rows.append(row)

        # delete result file
        if os.path.isfile(local_filename):
            os.remove(local_filename)

        return rows

if __name__ == '__main__':
    # SQL Query to execute
    query_string = ' '.join(sys.argv[1:]).replace('\r\n', '')
    query = (query_string)

    result = run_query(query, database, s3_ouput)
    pd.DataFrame(result).to_csv('temp.csv')

    print("Result is got")




