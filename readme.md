# Usage

1. Build and run the docker file
1. Exec into and run the following inside the container:

```shell
while true; do
  date
  echo
  curl localhost:5001/MemoryLeak
  echo
  echo
  ps aux
  echo
  sleep 5
done
```

You should see memory usage climb over time. For example:


```text
USER       PID %CPU %MEM    VSZ   RSS TTY      STAT START   TIME COMMAND
root         1  2.9  6.7 26223152 270856 pts/0 Ssl+ 16:39   0:09 dotnet DotNetNewRelicMemoryLeak.dll

... some 1 hour or so later ...

USER       PID %CPU %MEM    VSZ   RSS TTY      STAT START   TIME COMMAND
root         1  2.1 28.4 26976316 1146852 pts/0 Ssl+ 16:39   0:41 dotnet DotNetNewRelicMemoryLeak.dll
```

## Notes

I've noticed the memory usage will climb a bunch, then get collected, but shortly after continue to climb.
The collection cycles don't seem to lower the memory back to the original level, for example it'll go from 29% down to 22%, then climb well beyond up to 40% or more.
and then continue climbing again from there, instead of collecting all the way down to the original 8% or so the app has after the first few calls.

You can imagine the severity of the issue querying real production data if we see climbs to around 1/3 memory pressure on a simple selection of 4 rows of data.

You'll notice that changing `CORECLR_ENABLE_PROFILING` to 0 in the docker container will alleviate the memory issues almost entirely.
Memory will float around 9% instead of increasing indefinitely, at least from our testing.
