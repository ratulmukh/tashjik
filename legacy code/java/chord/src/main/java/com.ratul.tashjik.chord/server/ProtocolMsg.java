package com.ratul.tashjik.chord.server;

import java.util.UUID;

public class ProtocolMsg {
    public static class FindSucessorReq {
        public String sessionId;
        public int id;

        public FindSucessorReq(int id) {
            this.id = id;
            sessionId = UUID.randomUUID().toString();
        }
    }

    public static class FindSucessorResp {
        public String sessionId;
        public int id;
        public String successorIp;

        public FindSucessorResp(String sessionId, int id, String successorIp) {
            this.successorIp = successorIp;
            this.id = id;
            this.sessionId = sessionId;
        }
    }
}
