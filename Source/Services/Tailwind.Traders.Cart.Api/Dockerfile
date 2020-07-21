FROM node:9-alpine

WORKDIR /src

COPY package*.json ./
RUN apk --no-cache --virtual build-dependencies add \
    python \
    make \
    g++ \
    && npm install \
    && apk del build-dependencies

COPY . .

ENV PORT 3001
EXPOSE 3001
CMD [ "npm", "start" ]