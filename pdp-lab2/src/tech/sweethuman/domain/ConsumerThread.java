package tech.sweethuman.domain;

public class ConsumerThread extends Thread {
    private final Buffer buffer;
    private final Integer length;
    private Integer sum;

    public ConsumerThread(Buffer buffer, Integer length) {
        this.buffer = buffer;
        this.length = length;
        this.sum = 0;
    }

    @Override
    public void run() {
        for (var i = 0; i < length; i++) {
            var value = buffer.get();
            if (value == null) {
                System.out.println("Got null instead of a number");
                continue;
            }
            sum += value;
        }
        System.out.println("Finished summing up, final result " + sum.toString());
    }
}
