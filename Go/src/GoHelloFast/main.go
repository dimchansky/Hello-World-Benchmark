package main

import (
    "github.com/valyala/fasthttp"
)

const helloWorldString = "Hello, World!"

var helloWorldBytes = []byte(helloWorldString)

func plaintextHandler(ctx *fasthttp.RequestCtx) {
    ctx.Success("text/plain", helloWorldBytes)  
}

func main() {
    fasthttp.ListenAndServe(":8080", plaintextHandler)
}