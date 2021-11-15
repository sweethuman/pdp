package tech.sweethuman.model;

public class Pattern2Multiplication extends MatrixMultiplication {
    public Pattern2Multiplication(int[][] a, int[][] b, int size, int numberOfThreads) {
        super(a, b, size, numberOfThreads);
    }

    @Override
    public Runnable getRunnable() {
        return () -> {
            lock.lock();
            int core = step_i++;
            lock.unlock();
            for (int row_a_index = 0; row_a_index < size; row_a_index++) {
                for (int col_b_index = core * size / numberOfTasks; col_b_index < (core + 1) * size / numberOfTasks; col_b_index++) {
                    multiplySubTask(row_a_index, col_b_index);
                }
            }
        };
    }


}
