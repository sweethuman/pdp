package tech.sweethuman.model;

public class Pattern3Multiplication extends MatrixMultiplication {
    public Pattern3Multiplication(int[][] a, int[][] b, int size, int numberOfThreads) {
        super(a, b, size, numberOfThreads);
    }

    @Override
    public Runnable getRunnable() {
        return () -> {
            lock.lock();
            int core = step_i++;
            lock.unlock();
            for (int row_a_index = core; row_a_index < size; row_a_index += numberOfTasks) {
                for (int col_b_index = 0; col_b_index < size; col_b_index++) {
                    multiplySubTask(row_a_index, col_b_index);
                }
            }
        };
    }


}
