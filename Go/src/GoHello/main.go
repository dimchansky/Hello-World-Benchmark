package main

import (
	"io"
	"net/http"
	"runtime"
)

func plaintextHandler(w http.ResponseWriter, r *http.Request) {
	helloWorldString := "Hello, World!"
	w.Header().Set("Content-Type", "text/plain")
	io.WriteString(w, helloWorldString)
}

func main() {
	runtime.GOMAXPROCS(runtime.NumCPU())

	http.HandleFunc("/", plaintextHandler)
	http.ListenAndServe(":8080", nil)
}
