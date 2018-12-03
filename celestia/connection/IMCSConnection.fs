namespace Celestia

open System
open System.IO
open System.Net.Sockets

///Something went wrong while playing
exception PlayingError of string

///Something went wrong with the connection
exception ConnectionError of string

///Invalid protocol exception, for when unknown protocol events occur
exception ProtocolError of string

type CodeVal = 
            | InputError
            | Error of int
            | Command of int
            | Notification of int

///This type encapsulates a number of helper pieces for
///connecting to an IMCS server, namely the reader the writer
///the client and the network stream. Everything is wired up
///properly by the constructor
///this class implements IDisposable so as to be able to 
///clean up the underlying streams
type IMCSConnection(server, port) = 
        //the constructor just creates everything and
        //wraps everything
        let client = new TcpClient(server,port)
        let netstream = client.GetStream()
        let reader = new StreamReader(netstream)
        let writer = new StreamWriter(netstream)
        //autoflush is good
        do writer.AutoFlush <- true
        //this is a helper variable for IDisposed
        let mutable disposed = false

        //a helper function to dispose the underlying
        //streams, code heavily influenced by Don Symes,
        //Expert F#
        let cleanup() = if not disposed then
                            disposed <- true
                            client.Close() 
                            netstream.Dispose()
                            reader.Dispose()
                            writer.Dispose()
                        else ()

            
        ///The Dispose method destroys the underlying
        ///Streams
        interface IDisposable with
            member x.Dispose() = cleanup()
            

        ///returns the underlying netstream
        member x.NetStream with get() = netstream
        ///returns the underlying reader, note
        ///the reader does not have a time out
        //so some care must be taken when using this
        member x.Reader with get() = reader
        ///returns the underlying writer, autoflush is
        ///enabled
        member x.Writer with get() = writer
        ///returns the underlying TcpClient
        member x.Client with get() = client


