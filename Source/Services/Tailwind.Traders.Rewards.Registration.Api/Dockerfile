#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat 
ARG imageTag=4.7.2-windowsservercore-ltsc2019
FROM mcr.microsoft.com/dotnet/framework/wcf:${imageTag}
EXPOSE 808
WORKDIR /inetpub/wwwroot
COPY obj/Docker/publish .
