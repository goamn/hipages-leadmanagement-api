# hipages-leadmanagement-api

Requires .Net Core version 3.1 with Docker (if running Docker Toolbox, see the section about it below).

Build and then run the "LeadManagement.Api" project.

## Tests

Most of the tests are integration tests that depend on services that are spun up in docker.  To run the tests locally you will need to run the docker-compose first, run the following command(s):

```
cd ./src
docker-compose up -d
```

You can then run the tests with your preferred test runner, or using the standard:

```
dotnet test
```

## Docker Toolbox

If Docker for Windows can't be run (Windows 10 Home) and Docker Toolbox is running instead, port 16201 must be opened for localhost, required by postgres connections. Open virtualbox and go to the settings of the docker VM (should be named 'default').

```
Network -> Adapter 1 (NAT) -> Advanced -> Port Forwarding -> Add new:
```

| name  | protocol | host ip   | host port | guest ip | guest port |
| ----- |--------- | --------- | --------- | -------- | ---------- |
| nginx | TCP      | 127.0.0.1 | 16201     |          | 16201      |
