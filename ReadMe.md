### .Net Platform 6 Service

Simple ASP.NET App using the Kestrel Web Server



When on MacOS I build a distro for Linux using:

```
dotnet publish -o dist/  -r linux-x64 platform6-service.csproj
```

I zip the folder and transfer to the p6core instance and expand into:

- /opt/service.net/dist

Also need to copy the javascript bundle to:

- /opt/service.net/wwwroot

I start with a `runit.sh` script in /opt/service.net:

```
#!/bin/bash
#set -x

export EXTERNAL_URL=http://dev.internal.sidetrade.io:9192
export UI_BUNDLE_PATH=./wwwroot/ServiceConfiguration.bundle.js

nohup ./dist/platform6-service --urls="http://0.0.0.0:9192" > p6service.log &
```

#### Example Call From P6 Groovy Script

```groovy
def cm = [
    headers: [
        'crypto.id': 'bitcoin',
        'demo.net.action': 'price'
    ]
]

def cmResponse = p6.service.request("demo.net", cm)
println cmResponse
```


//TODO

 - the P6 Proxy URL and EXTERNAL_URL value is hardcoded in the ServiceConfiguration.bundle.js


Note:

You must keep the Hazelcast version compatibe with p6core (currently: 3.21.x)