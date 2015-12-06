using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSP;

namespace simulated_annealing_group_project {
class Tour {
    public List<City> cities = new List<City>();
    public City[] cityArray;

    public Tour(City[] cities) {
        cityArray = cities;
        this.cities = cities.ToList();
    }

    // could be the length of the tour
    public double getEnergy() {
        double tourLength = 0;
        for (int i = 0; i < cities.Count() - 1; i++) {
            var from = cities[i];
            var to = cities[i + 1];
            tourLength = tourLength + from.costToGetTo(to);
        }

        tourLength = tourLength + cities.Last().costToGetTo(cities[0]);
        return tourLength;
    }

    public Tuple<int, int> getLongestEdge() {
        int from = 0;
        int to = 0;
        double longestEdge = 0;

        for (int i = 0; i < cities.Count() - 1; i++) {
            var fromCity = cities[i];
            var toCity = cities[i + 1];
            var cost = fromCity.costToGetTo(toCity);
            if (cost > longestEdge) {
                from = i;
                to = i + 1;
                longestEdge = cost;
            }
        }

        return new Tuple<int,int>(from, to);
    }

    public void swapCities(int a, int b) {
        var temp = cities[a];
        cities[a] = cities[b];
        cities[b] = temp;
    }

    public ArrayList toArrayList() {
        ArrayList output = new ArrayList();

        foreach (var city in cities) {
            output.Add(city);
        }

        return output;
    }
}
}
