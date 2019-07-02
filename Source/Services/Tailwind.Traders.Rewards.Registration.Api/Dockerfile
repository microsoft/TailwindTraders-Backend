#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat 

FROM mcr.microsoft.com/dotnet/framework/wcf:4.7.2-windowsservercore-ltsc2019
ARG source
EXPOSE 808
WORKDIR /inetpub/wwwroot
COPY ${source:-obj/Docker/publish} .
