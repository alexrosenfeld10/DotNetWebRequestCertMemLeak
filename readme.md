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

The output of the memory info endpoint looks something like this. Pay close attention to `privateMemorySize`:
```json
{
  "instanceId": "boxer",
  "totalMemory": "55.666 MB",
  "totalAllocated": "77.724 MB",
  "gen1CollectionCount": 0,
  "gen2CollectionCount": 0,
  "gen3CollectionCount": 0,
  "heapSize": "8.521 MB",
  "memoryLoad": "8.443 GB",
  "highMemoryLoadThreshold": "14.073 GB",
  "fragmented": "1.479 MB",
  "totalAvailableMemory": "15.637 GB",
  "handles": 205,
  "pagedMemorySize64": "8 KB",
  "workingSet": "829.707 MB",
  "privateMemorySize": "1.037 GB",
  "virtualMemorySize": "20.002 GB",
  "peakVirtualMemorySize": "20.064 GB",
  "peakWorkingSet": "829.707 MB"
}
```

6. To reconfigure ServicePointManager hit the `/MemoryLeak` endpoint. This will set `ServicePointManager.CheckCertificateRevocationList = true;`:

```shell
curl -v http://localhost:5001/MemoryLeak/
```

7. To force a GC (to ensure that accumulated memory isn't GC-able) hit the `/MemoryLeak/gccollect` endpoint:
```shell
curl -v http://localhost:5001/MemoryLeak/gccollect
```
