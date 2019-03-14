FROM golang:alpine

WORKDIR /go/src/app
# RUN apk add --no-cache git
RUN apk update && apk add git 


ENV GO111MODULE=on
COPY Services/Tailwind.Traders.PopularProduct.Api/go.mod .
COPY Services/Tailwind.Traders.PopularProduct.Api/go.sum .

RUN go mod download

COPY Services/Tailwind.Traders.PopularProduct.Api .

RUN go get .
# RUN go build -o main .
RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build -a -installsuffix cgo -o main .

EXPOSE 80
CMD ["./main"]