module Main where

import Network.Wai ( Application, Response( ResponseBuilder ) )
import Network.HTTP.Types ( status200 )
import Network.HTTP.Types.Header ( hContentType, hContentLength, hConnection )
import Network.Wai.Handler.Warp ( run )
import Blaze.ByteString.Builder (fromByteString)
import qualified Data.ByteString.Char8 as BS ( pack, length )

application :: Application
application _ = return $ 
    ResponseBuilder status200 [(hContentType, BS.pack "text/plain"), 
                               (hContentLength, BS.pack bodyLen ), 
                               (hConnection, BS.pack "keep-alive")] 
                    $ fromByteString body
    where body = BS.pack "Hello, World!"
          bodyLen = show . BS.length $ body

main :: IO ()
main = run 8080 application