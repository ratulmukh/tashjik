package com.ratul.tashjik.chord.server;

import com.esotericsoftware.kryo.Kryo;
import com.esotericsoftware.kryonet.Client;
import com.esotericsoftware.kryonet.Connection;
import com.esotericsoftware.kryonet.Listener;

import java.io.IOException;

public class KryoClient {
    String ipAddress;
    Client client;

    public KryoClient(String ipAddress) throws IOException {
        this.ipAddress = ipAddress;
        connectToRealNode();
    }

    private void connectToRealNode() throws IOException {
        client = new Client();

        Kryo kryo = client.getKryo();
        kryo.register(ProtocolMsg.FindSucessorReq.class);
        kryo.register(ProtocolMsg.FindSucessorResp.class);

        client.start();
        client.connect(5000, ipAddress, 54555, 54777);
    }

    public void sendMsg(Object object) {
        client.sendTCP(object);

        client.addListener(new Listener() {
            public void received (Connection connection, Object object) {
                if (object instanceof ProtocolMsg.FindSucessorResp) {
                    ProtocolMsg.FindSucessorResp findSucessorResp = (ProtocolMsg.FindSucessorResp)object;
                    System.out.println(findSucessorResp.sessionId);
                }
            }
        });
    }
}
