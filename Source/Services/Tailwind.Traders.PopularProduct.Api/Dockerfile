FROM golang:alpine 
COPY Services/Tailwind.Traders.PopularProduct.Api /go/src/app
WORKDIR /go/src/app
RUN apk update && apk add git 
RUN go get .
RUN go build -o main .
EXPOSE 80
CMD ["./main"]