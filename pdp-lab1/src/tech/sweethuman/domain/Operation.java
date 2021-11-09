package tech.sweethuman.domain;

import java.text.MessageFormat;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

public class Operation {
    public int src;
    public int dest;
    public int amount;
    public LocalDateTime timestamp;

    public Operation(int src, int dest, int amount, LocalDateTime timestamp) {
        this.src = src;
        this.dest = dest;
        this.amount = amount;
        this.timestamp = timestamp;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        Operation operation = (Operation) o;
        return src == operation.src &&
                dest == operation.dest &&
                amount == operation.amount &&
                timestamp.equals(operation.timestamp);
    }

    @Override
    public String toString() {
        return MessageFormat.format("Operation: {0}$ sent from {1} to {2} at {3}", amount, src, dest, DateTimeFormatter.ofPattern("HH:mm:ss:SS").format(timestamp));
    }
}
