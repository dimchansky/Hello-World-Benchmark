package main

import (	
	"net/http"
	"runtime"
)

const helloWorldString = "Hello, World!"

var helloWorldBytes = []byte(helloWorldString)

func plaintextHandler(w http.ResponseWriter, r *http.Request) {	
	w.Header().Set("Content-Type", "text/plain")
	w.Write(helloWorldBytes)
}

func main() {
	runtime.GOMAXPROCS(runtime.NumCPU())

	http.HandleFunc("/", plaintextHandler)
	http.ListenAndServe(":8080", nil)
}
