FROM node:8.12.0-alpine

WORKDIR /src

COPY /Services/Tailwind.Traders.Coupon.Api/package*.json ./
RUN npm install
COPY /Services/Tailwind.Traders.Coupon.Api/ .

EXPOSE 3000
CMD [ "node", "server.js" ]