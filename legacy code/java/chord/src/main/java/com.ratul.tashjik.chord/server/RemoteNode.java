package com.ratul.tashjik.chord.server;

import com.ratul.tashjik.chord.server.Node;

import java.util.concurrent.CompletableFuture;

public class RemoteNode implements Node {
    String ipAddress;

    public RemoteNode(String ipAddress) {
        this.ipAddress = ipAddress;
    }

    @Override
    public int getId() {
        return 0;
    }

    @Override
    public boolean isInInterval(Node node, int n) {
        return false;
    }

    @Override
    public boolean isInInterval(Node node1, Node node2) {
        return false;
    }

    @Override
    public CompletableFuture<Node> findSuccessorAsync(int id) {
        return null;
    }

    @Override
    public Boolean store(String key, String value) {
        return false;
    }
}
