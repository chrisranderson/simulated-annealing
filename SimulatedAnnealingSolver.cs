using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TSP;

namespace simulated_annealing_group_project {
class SimulatedAnnealingSolver {
    public Random rand = new Random();
    public bool printDebug = true;
    public bool saveDebug = true;
    public string log = "";
    public double TEMP_INTERVAL = -.4;
    public int SWAP_COUNT = 3;
    public double TEMPERATURE_START_MULT = .5;
    public Stopwatch timer = new Stopwatch();
    public int TIME_LIMIT = 30;

    public void debug(string x, object y) {
        var output = x + ": " + y.ToString() + "\n";
        
        if (printDebug) {
            Console.Write(output);
        } 
    }

    public void debug(string x) {
        debug(x, "n/a");
    }

    public ArrayList solve(City[] cities) {
        var currentTour = getInitialTour(cities);
        var bestTour = currentTour;
        var interval = .5;
        var updatedTours = new List<double>();
        double mimimumTour = double.PositiveInfinity;

        Console.WriteLine("==========================");
        Console.WriteLine("==========================");
        Console.WriteLine("==========================");
        Console.WriteLine("==========================");
        Console.WriteLine("==========================");

        // solve a bunch of times with different parameters
        // .5 empirical best for 5 <= n <= 20
        //for (double interval = .5; interval <= .5; interval += .2) {
        // instead of taking less than, take the minimum of all tours ever generated
        // stop searching after a certain amount of changes
        timer.Start();

       

            for (int numSwaps = 3; numSwaps < 6; numSwaps += 1) {
                currentTour = getInitialTour(cities);

                int stepCount = 0;
                int totalIterations = 0;
                var lastSave = 0;
                // We need to figure out what this should actually be.
                double temperature = currentTour.getEnergy()*TEMPERATURE_START_MULT;
                //double temperature = 200;
                //debug("initial temp", temperature);
                while (temperature > .01) {
                    totalIterations++;
                    stepCount++;

                    if (timer.Elapsed.TotalSeconds > TIME_LIMIT) {
                        //break;
                    }
                    //debug("================");
                    temperature = (temperature * .9999);
                    //temperature = (initialTemp / Math.Log10(totalIterations)) - (initialTemp / 7);
                    //temperature = getTemperature(temperature, .5);
                    numSwaps = (int) Math.Max(Math.Floor(Math.Sqrt(temperature)), 1);
                    var neighbor = getNeighbor(currentTour, numSwaps);
                    var currentEnergy = currentTour.getEnergy();
                    var neighborEnergy = neighbor.getEnergy();
                    //debug("current energy", currentTour.getEnergy());
                    //debug("neighbor energy", neighbor.getEnergy());
                    if (shouldAcceptNeighbor(currentEnergy, neighborEnergy, temperature)) {
                        //debug("accepted neighbor");
                        currentTour = neighbor;
                        updatedTours.Add(neighborEnergy);
                        if (neighborEnergy < mimimumTour) {
                            lastSave = stepCount;
                            stepCount = 0;
                            bestTour = currentTour;
                            mimimumTour = neighborEnergy;
                            debug("==== IMPROVED ====");
                            debug("timer", timer.Elapsed.TotalSeconds);
                            debug("step count", lastSave);
                            debug("tour length", neighborEnergy);
                            debug("temp interval", interval);
                            debug("number of swaps", numSwaps);
                            timer.Restart();
                        }
                    } else {
                        //temperature *= 1.01;
                    }
                }

                if (timer.Elapsed.TotalSeconds > TIME_LIMIT) {
                    break;
                }

            }
        //}

        return bestTour.toArrayList();
    }

    public int getMinSwaps(int numberOfCities) {
        int half = (int) Math.Floor((decimal) numberOfCities / 2);
        return Math.Max(0, half - 5);
    }

    public int getMaxSwaps(int numberOfCities) {
        int half = (int)Math.Floor((decimal)numberOfCities / 2);
        return Math.Max(0, half + 5);
    }

    // Read somewhere to start at an arbitrary initial state
    // Maybe we could be creative with this
    public Tour getInitialTour(City[] cityArray) {
        return new Tour(cityArray);
    }

    // Could be a function of the number of steps we've taken
    // 
    public double getTemperature(double oldTemperature, double interval) {
        // This function body needs to change.
        return oldTemperature - interval;
    }

    // TODO: replace function body
    // could find the longest edge in a tour or something
    // seems swapping 3 random cities does better than 1
    public Tour getNeighbor(Tour currentTour, int numSwaps) {
        var output = new Tour(currentTour.cityArray);


        for (var i = 0; i < numSwaps; i++) {
            swapTwoRandomCities(output);
            swapRandomWithAdjacent(output);
        }

        //wesAlgorithm(output);

        //var longestEdge = output.getLongestEdge();
        //swapLongestEdgeCitiesWithRandom(output, longestEdge);
        //longestEdge = output.getLongestEdge();
        //swapLongestEdgeCitiesWithRandom(output, longestEdge);
        //longestEdge = output.getLongestEdge();
        //swapLongestEdgeCitiesWithRandom(output, longestEdge);
       
        return output;
    }

    public int circularIndex(int index, int size) {
        if (index >= 0) {
            return index % size;
        } else {
            return index + size;
        }
    }

    public void wesAlgorithm(Tour tour) {
        var size = tour.cities.Count();
        var swapCityLocation = rand.Next(0, size);
        var loopSize = rand.Next(1, 5);

        for (int i = 1; i <= loopSize; i++) {
            tour.swapCities(circularIndex(swapCityLocation + i, size), 
                            circularIndex(swapCityLocation - i, size));
        }
    }

    public void swapTwoRandomCities(Tour tour) {
        var swapCityLocation = rand.Next(0, tour.cities.Count());
        var otherCity = rand.Next(0, tour.cities.Count());
        tour.swapCities(swapCityLocation, otherCity);
    }

    public void swapRandomWithAdjacent(Tour tour) {
        var adjacentOne = rand.Next(0, tour.cities.Count() - 1);
        tour.swapCities(adjacentOne, adjacentOne + 1);
    }

    public void swapLongestEdgeCitiesWithRandom(Tour tour, Tuple<int, int> edge) {
        var swapCityLocation = rand.Next(0, tour.cities.Count());
        var swapCityLocation2 = rand.Next(0, tour.cities.Count());
        tour.swapCities(edge.Item1, swapCityLocation);
        tour.swapCities(edge.Item2, swapCityLocation2);
    }

    public void swapLongestWithAdjacent(Tour tour, Tuple<int, int> edge) {
        var from = edge.Item1;
        if (from != 0) {
            tour.swapCities(from, from - 1);
        }
    }

    // TODO: replace function body
    public bool shouldAcceptNeighbor(double currentEnergy, double neighborEnergy, double temperature) {
        //debug("current energy", currentEnergy);
        double randomNumber = this.rand.NextDouble();
        double probability;

        // https://en.wikipedia.org/wiki/Simulated_annealing 
        // look under "acceptance probabilities
        if (neighborEnergy < currentEnergy) {
            probability = 1;
        } else {
            probability = Math.Exp(-(neighborEnergy - currentEnergy) / temperature);
        }

        //debug("temperature", temperature);
        //debug("probability", probability);
        return probability > randomNumber;
    }
}
}
