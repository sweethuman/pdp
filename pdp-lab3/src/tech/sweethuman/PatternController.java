package tech.sweethuman;

import tech.sweethuman.model.MatrixMultiplication;
import tech.sweethuman.model.Pattern1Multiplication;
import tech.sweethuman.model.Pattern2Multiplication;
import tech.sweethuman.model.Pattern3Multiplication;

import java.io.BufferedWriter;
import java.io.FileWriter;
import java.io.IOException;
import java.text.MessageFormat;
import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.*;


public class PatternController {
    private final int[][] a;
    private final int[][] b;
    private final int size;
    private final int numberOfThreads;
    private final int numberOfTasks;

    public PatternController(int[][] a, int[][] b, int size, int numberOfThreads, int numberOfTasks) {
        this.a = a;
        this.b = b;
        this.size = size;
        this.numberOfThreads = numberOfThreads;
        this.numberOfTasks = numberOfTasks;
    }

    protected void lowLevelPatternRunner(MatrixMultiplication matrixMultiplication, String patternName) throws IOException, InterruptedException {
        var startTime = LocalDateTime.now();

        List<Thread> threads = new ArrayList<>();
        // Create threads
        for (int i = 0; i < numberOfThreads; i++) {
            threads.add(new Thread(matrixMultiplication.getRunnable()));
        }

        // Join threads
        for (Thread thread : threads) {
            thread.start();
            System.out.println("started");
        }

        for (Thread thread : threads) {
            thread.join();
            System.out.println("joined");
        }

        var endTime = LocalDateTime.now();
        String message = MessageFormat.format("{0}: {1}; SIZE: {2}; THREADS: {3}; TASKS: {4} \n", patternName, ChronoUnit.MILLIS.between(startTime, endTime), size, numberOfThreads, matrixMultiplication.getNrOfTasks());
        writeToFile(message);
    }

    public void lowLevelPattern1() throws InterruptedException, IOException {
        var matrixMultiplication = new Pattern1Multiplication(a, b, size, numberOfThreads);
        lowLevelPatternRunner(matrixMultiplication, "LOW LEVEL PATTERN 1");
    }

    public void lowLevelPattern2() throws InterruptedException, IOException {
        var matrixMultiplication = new Pattern2Multiplication(a, b, size, numberOfThreads);
        lowLevelPatternRunner(matrixMultiplication, "LOW LEVEL PATTERN 2");
    }

    public void lowLevelPattern3() throws InterruptedException, IOException {
        var matrixMultiplication = new Pattern3Multiplication(a, b, size, numberOfThreads);
        lowLevelPatternRunner(matrixMultiplication, "LOW LEVEL PATTERN 3");
    }

    protected void threadPoolPatternRunner(MatrixMultiplication matrixMultiplication, String patternName) throws IOException, InterruptedException, ExecutionException {
        ExecutorService executorService = Executors.newFixedThreadPool(numberOfThreads);

        var startTime = LocalDateTime.now();
        var tasks = new ArrayList<Callable<Object>>();
        for (int i = 0; i < matrixMultiplication.getNrOfTasks(); i++) {
            tasks.add(Executors.callable(matrixMultiplication.getRunnable()));
        }
        System.out.println("started tasks");
        executorService.invokeAll(tasks);
        System.out.println("finished tasks");
        var endTime = LocalDateTime.now();

        String message = MessageFormat.format("{0}: {1}; SIZE: {2}; THREADS: {3}; TASKS: {4} \n", patternName, ChronoUnit.MILLIS.between(startTime, endTime), size, numberOfThreads, matrixMultiplication.getNrOfTasks());
        writeToFile(message);
        executorService.shutdown();
    }

    public void threadPoolPattern1() throws IOException, InterruptedException, ExecutionException {
        MatrixMultiplication matrixMultiplication = new Pattern1Multiplication(a, b, size, numberOfTasks);
        threadPoolPatternRunner(matrixMultiplication, "THREAD POOL PATTERN 1");
    }

    public void threadPoolPattern2() throws IOException, InterruptedException, ExecutionException {
        MatrixMultiplication matrixMultiplication = new Pattern2Multiplication(a, b, size, numberOfTasks);
        threadPoolPatternRunner(matrixMultiplication, "THREAD POOL PATTERN 2");
    }

    public void threadPoolPattern3() throws IOException, InterruptedException, ExecutionException {
        MatrixMultiplication matrixMultiplication = new Pattern3Multiplication(a, b, size, numberOfTasks);
        threadPoolPatternRunner(matrixMultiplication, "THREAD POOL PATTERN 3");
    }

    public void writeToFile(String text) throws IOException {
        BufferedWriter writer = new BufferedWriter(
                new FileWriter("./report.txt", true));

        writer.write(text);
        writer.close();
    }
}
