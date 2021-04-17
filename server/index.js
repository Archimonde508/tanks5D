let Server = require('./Classes/Server')
let server = new Server(process.env.PORT == undefined);

const WebSocket = require('ws')
const wss = new WebSocket.Server({ port: 3000 },()=>{
    console.log('server started')
})
wss.on('connection', (ws) => {
    let connection = server.onConnected(ws);
    ws.send(JSON.stringify({method: "init", message: "{}"}))
    // console.log(connection)
    ws.on('message', (data) => {
        server.handleMessage(ws, data);
        console.log('data received \n %o',data)
        // ws.send(data);
    })

    ws.on('listening',()=>{
        console.log('listening on 3000')
    })

    ws.on('disconnect', (ws)=>{
        console.log('disconnected');
    }) 
    
})
