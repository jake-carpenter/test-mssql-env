#!/bin/bash

test-mssql-env destroy -c settings.json
test-mssql-env init -c settings.json
test-mssql-env create-db -c settings.json

