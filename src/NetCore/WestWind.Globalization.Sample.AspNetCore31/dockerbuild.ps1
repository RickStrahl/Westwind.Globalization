# publish the application first
dotnet publish -c Release

# clean up old image and any containers (running or not)
docker stop westwindglobalization
docker rm westwindglobalization -f 
docker rmi rickstrahl/westwindglobalization:westwindglobalization

# create new image
docker build -t rickstrahl/westwindglobalization:westwindglobalization .

# immediately start running the container in the background (-d) (no console)
docker run  -it -p 5004:80 --name westwindglobalization  rickstrahl/westwindglobalization:westwindglobalization 

# Map host IP to a domain - so we can access local SQL server
# $localIpAddress=((ipconfig | findstr [0-9].\.)[0]).Split()[-1]
#--add-host dev.west-wind.com:$localIpAddress

#docker stop westwindglobalization
#docker rm westwindglobalization

# docker exec -it westwindglobalization  /bin/bash

# # if above doesn't work
# docker exec -it westwindglobalization  /bin/sh

#docker push 
