FROM mcr.microsoft.com/mssql-tools as mssql-tools-with-gettext
RUN apt-get update && apt-get -y install gettext-base

FROM mssql-tools-with-gettext
WORKDIR /scripts
COPY scripts .
RUN chmod +x run.sh
CMD  /scripts/run.sh 

