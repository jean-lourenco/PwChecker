#!/usr/bin/env bash

#exit if any command fails
set -e

dotnet restore
dotnet build

#dotnet test -c Release

revision=${TRAVIS_JOB_ID:=1}
revision=$(printf "%04d" $revision)

