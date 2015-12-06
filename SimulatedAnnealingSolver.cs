using System;
using System.Collections;
using System.Collections.Generic;
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
    public double TEMPERATURE_START_MULT = 1;

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

        Console.WriteLine("==========================");
        Console.WriteLine("==========================");
        Console.WriteLine("==========================");
        Console.WriteLine("==========================");
        Console.WriteLine("==========================");

        // solve a bunch of times with different parameters
        // .5 empirical best for 5 <= n <= 20
        for (double interval = .5; interval <= .5; interval += .2) {
            for (int numSwaps = 7; numSwaps < 15; numSwaps += 1) {
                currentTour = getInitialTour(cities);
                var steps = 0;

                // We need to figure out what this should actually be.
                double temperature = currentTour.getEnergy()*TEMPERATURE_START_MULT;
                //debug("initial temp", temperature);
                while (temperature > 0) {
                    //debug("================");
                    temperature = getTemperature(temperature, interval);
                    var neighbor = getNeighbor(currentTour, numSwaps);
                    //debug("current energy", currentTour.getEnergy());
                    //debug("neighbor energy", neighbor.getEnergy());
                    if (shouldAcceptNeighbor(currentTour.getEnergy(), neighbor.getEnergy(), temperature)) {
                        //debug("accepted neighbor");
                        currentTour = neighbor;
                    }
                }

                if (currentTour.getEnergy() < bestTour.getEnergy()) {
                    debug("==== IMPROVED ====");
                    debug("tour length", currentTour.getEnergy());
                    debug("temp interval", interval);
                    debug("number of swaps", numSwaps);
                    bestTour = currentTour;
                } 
            }
        }

        return bestTour.toArrayList();
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

        var longestEdge = output.getLongestEdge();
        swapLongestEdgeCitiesWithRandom(output, longestEdge);
        //longestEdge = output.getLongestEdge();
        //swapLongestEdgeCitiesWithRandom(output, longestEdge);
        //longestEdge = output.getLongestEdge();
        //swapLongestEdgeCitiesWithRandom(output, longestEdge);
       
        return output;
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
