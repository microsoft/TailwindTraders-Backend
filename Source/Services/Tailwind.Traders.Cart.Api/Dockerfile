FROM node:8.12.0-alpine

WORKDIR /src

COPY package*.json ./
RUN npm install
COPY . .

ENV PORT 3001
EXPOSE 3001
CMD [ "npm", "start" ]