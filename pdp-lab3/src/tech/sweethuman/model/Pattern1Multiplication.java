package tech.sweethuman.model;

public class Pattern1Multiplication extends MatrixMultiplication {
    public Pattern1Multiplication(int[][] a, int[][] b, int size, int numberOfThreads) {
        super(a, b, size, numberOfThreads);
    }

    @Override
    public Runnable getRunnable() {
        return () -> {
            lock.lock();
            int core = step_i++;
            lock.unlock();
            for (int row_a_index = core * size / numberOfTasks; row_a_index < (core + 1) * size / numberOfTasks; row_a_index++) {
                for (int col_b_index = 0; col_b_index < size; col_b_index++) {
                    multiplySubTask(row_a_index, col_b_index);
                }
            }
        };
    }
}
