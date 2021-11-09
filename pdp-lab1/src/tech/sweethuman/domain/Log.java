package tech.sweethuman.domain;

import java.util.ArrayList;
import java.util.List;

public class Log {
    public List<Operation> operations;

    public Log() {
        operations = new ArrayList<>();
    }

    public void addToLog(Operation operation) {
        operations.add(operation);
    }
}
