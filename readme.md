# Usage

Build and run the docker file or build and run the console app

```shell
docker build --network host -t dotnet-new-relic-memory-leak .
docker run --network host --rm dotnet-new-relic-memory-leak

# OR

dotnet run
```

The app will log iterations of using `WebRequest` to make requests to
`collector.newrelic.com`, a host that recently had a certificate expire.

After 20 seconds the app will reconfigure such that
`ServicePointManager.CheckCertificateRevocationList = true`. Oddly, it has been
my experience that if this setting is set immediately there isn't a memory
leak.

At this point the RSS memory should balloon noticeably and continue to grow with
subsequent requests.

Since reworking this as a console app, I see the memory hit a steady state
around 250MB. When this bug was part of an ASP.NET web app, the memory could
easily increase over 1GB. Not sure what is different. See the git history if
you want to take the web app version for a spin.
