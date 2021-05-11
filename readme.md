# Usage

1. Update the Dockerfile to include a real `NEW_RELIC_LICENSE_KEY`

2. Build and run the docker file

```shell
docker build --network host -t dotnet-new-relic-memory-leak .
docker run --network host --rm dotnet-new-relic-memory-leak
```

3. Exec into the container

```shell
# Something like this should work if you only have one container running.
# Otherwise you'll need to look up the container name with `docker ps`
docker exec -it --privileged "$(docker ps | tail -1 | awk '{print $1}')" bash
```

4. Run something like the following inside the container to make sure new relic is working correctly:

```shell
tail -f /app/newrelic/logs/newrelic_agent_DotNetNewRelicMemoryLeak.log
```

5. To get memory info from the container you can hit the /MemoryLeak/memoryinfo endpoint from outside the container:

```shell
while true; do date; curl -v http://localhost:5001/MemoryLeak/memoryinfo | jq .; sleep 5; done
```

6. To reconfigure ServicePointManager hit the `/MemoryLeak` endpoint. This will set `ServicePointManager.CheckCertificateRevocationList = true;`:

```shell
curl -v http://localhost:5001/MemoryLeak/
```

7. To force a GC (to ensure that accumulated memory isn't GC-able) hit the `/MemoryLeak/gccollect` endpoint:
```shell
curl -v http://localhost:5001/MemoryLeak/gccollect
```
