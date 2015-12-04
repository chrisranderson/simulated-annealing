using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP {
    class GreedySolver {
        public static ArrayList solve(City[] cityArray) {
            return solve(cityArray, 0);
        }

        public static ArrayList solve(City[] cityArray, int startingPoint) {
            List<City> cities = cityArray.ToList();
            ArrayList solution = new ArrayList();

            var firstCity = cities[startingPoint];
            solution.Add(firstCity);
            cities.RemoveAt(startingPoint);

            var currentCity = firstCity;
            while (cities.Count > 0) {
                var bestCity = removeClosestCity(currentCity, cities);
                solution.Add(bestCity);
                currentCity = bestCity;
            }

            return solution;
        }

        private static City removeClosestCity(City current, List<City> cities) {
            var closestDistance = double.PositiveInfinity;
            City closestCity = new City(-1, -1);
            var cityIndex = -1;

            for (int i = 0; i < cities.Count; i++) {
                var testCity = cities[i];
                var testDistance = current.costToGetTo(testCity);
                if (testDistance < closestDistance) {
                    closestCity = testCity;
                    closestDistance = testDistance;
                    cityIndex = i;
                }
            }

            cities.RemoveAt(cityIndex);

            return closestCity;
        }

    }
}
