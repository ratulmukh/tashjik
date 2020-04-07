package com.ratul.tashjik.chord.server;

import org.apache.commons.lang3.tuple.Pair;

import java.util.Random;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;

public class LocalNode implements Node {
    public final int ID_BOUND = 2000000;
    RemoteNode bootstrapNode;
    public final int id;
    private int m = 5;
    Finger[] fingerTable = new Finger[m];
    Node predecessor;

    public LocalNode(RemoteNode bootstrapNode) throws ExecutionException, InterruptedException {
        this.bootstrapNode = bootstrapNode;
        id = (new Random()).nextInt(ID_BOUND);

        join(bootstrapNode);
    }

    public int getId() {
        return id;
    }

    public CompletableFuture<Node> findSuccessorAsync(int id) {
        return null;
    }

    public Node findPredecessor(int id) {
        Node predecessor = this;

        return null;
    }

    public Node closestPreceedingFinger(int id) {
        for(int i=m-1; m>=0; m--) {
            if(fingerTable[i].node.isInInterval(this, id)) {
                return fingerTable[i].node;
            }
        }
        return this;
    }

    public boolean isInInterval(Node node, int n) {
        if(node.getId()>this.id && this.id<n) {
            return true;
        } else {
            return false;
        }
    }

    public boolean isInInterval(Node node1, Node node2) {
        if(node1.getId()>this.id && this.id<node2.getId()) {
            return true;
        } else {
            return false;
        }
    }

    private void initFingerTable(RemoteNode bootstrapNode) {

    }

    private void updateOthers() {

    }

    private void join(RemoteNode bootstrapNode) throws ExecutionException, InterruptedException {
        if(bootstrapNode != null) {
            CompletableFuture.completedFuture(bootstrapNode)
                    .thenAcceptAsync(this::initFingerTable)
                    .thenRunAsync(this::updateOthers)
                    .get();
        } else {
            for (int i =0; i<m; i++) {
                fingerTable[i].node = this;
            }
            predecessor = this;
        }
    }

    public static class Finger {
        int start;
        Pair<Integer, Integer> interval;
        Node node, successor, predecessor;
    }

    @Override
    public Boolean store(String key, String value) {
        return false;
    }
}
