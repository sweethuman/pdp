package tech.sweethuman;

import java.io.IOException;
import java.util.Random;
import java.util.concurrent.ExecutionException;


public class Main {

    public static void main(String[] args) throws InterruptedException, IOException, ExecutionException {
        // Necessary parameters
        int min = 1;
        int max = 10;
        int SIZE_OF_MATRIX = 2000;
        int NUMBER_OF_THREADS = 10;
        int NUMBER_OF_TASKS = 20;

        // Generate 2 random matrices
        int[][] a = new int[SIZE_OF_MATRIX][SIZE_OF_MATRIX];
        int[][] b = new int[SIZE_OF_MATRIX][SIZE_OF_MATRIX];
        var r = new Random();
        for (int i = 0; i < SIZE_OF_MATRIX; i++) {
            for (int j = 0; j < SIZE_OF_MATRIX; j++) {
                a[i][j] = r.nextInt(min, max);
                b[i][j] = r.nextInt(min, max);
            }
        }
        System.out.println("finished matrix init");
        PatternController patternController = new PatternController(a, b, SIZE_OF_MATRIX, NUMBER_OF_THREADS, NUMBER_OF_TASKS);
        System.out.println("finished init");
        // PATTERN 1 - LOW LEVEL
        patternController.lowLevelPattern1();

        // PATTERN 2 - LOW LEVEL
        patternController.lowLevelPattern2();

        // PATTERN 3 - LOW LEVEL
        patternController.lowLevelPattern3();

        // PATTERN 1 - THREAD POOL
        patternController.threadPoolPattern1();

        // PATTERN 2 - THREAD POOL
        patternController.threadPoolPattern2();

        // PATTERN 3 - THREAD POOL
        patternController.threadPoolPattern3();

        patternController.writeToFile("\n");
    }

}
