{-# LANGUAGE OverloadedStrings #-}

module Main where

import qualified Data.ByteString.Builder   as B
import qualified Data.ByteString.Char8     as BS
import           Network.HTTP.Types        (status200)
import           Network.HTTP.Types.Header (hConnection, hContentLength,
                                            hContentType)
import           Network.Wai               (Application, responseBuilder)
import           Network.Wai.Handler.Warp  (run)

application :: Application
application _ respond = respond $
    responseBuilder status200 headers $ B.byteString body
    where headers = [(hContentType, "text/plain"),
                     (hContentLength, bodyLen ),
                     (hConnection, "keep-alive")]
          body = "Hello, World!"
          bodyLen = BS.pack . show . BS.length $ body

main :: IO ()
main = run 8080 application
