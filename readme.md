# Usage

1. Build and run the docker file

```shell
docker build --network host -t dotnet-new-relic-memory-leak .
docker run --network host --rm dotnet-new-relic-memory-leak
```

2. To get memory info from the container you can hit the `/MemoryLeak/memoryinfo` endpoint from outside the container:

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

3. To make some requests with the app hit the `/MemoryLeak/makerequest` endpoint. **The request will fail with a 500**. It doesn't need to work to reveal the problem. I'd suggest making a few such requests until you are comfortable that the app is in a stable place with regard to memory.

4. To reconfigure ServicePointManager hit the `/MemoryLeak` endpoint. This will set `ServicePointManager.CheckCertificateRevocationList = true;`:

```shell
curl -v http://localhost:5001/MemoryLeak/
```

After executing this endpoint, subsequent requests to `/MemoryLeak/makerequest` will cause the `privateMemorySize` to grow noticeably.

5. To force a GC (to ensure that accumulated memory isn't GC-able) hit the `/MemoryLeak/gccollect` endpoint:
```shell
curl -v http://localhost:5001/MemoryLeak/gccollect
```
