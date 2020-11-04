using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputProcessor : MonoBehaviour
{
    public GameObject algorithm_params;
    public GameObject algorithm_info;

    public InputField population_size;
    public InputField chromosome_length;
    public InputField crossover_rate;
    public InputField mutation_rate;
    public InputField satisfactory_fitness_level;
    public Toggle collissions;

    public Text LogText;
    public Text population_size_text;
    public Text chromosome_length_text;
    public Text crossover_rate_text;
    public Text mutation_rate_text;

    public GridManager grid_manager;

    public void CheckInputsAndStartGeneticAlgorithm()
    {
        try
        {
            grid_manager.genetic_algorithm.population_size = int.Parse(population_size.text);
            grid_manager.genetic_algorithm.chromosome_length = int.Parse(chromosome_length.text);
            grid_manager.genetic_algorithm.crossover_probability = float.Parse(crossover_rate.text);
            grid_manager.genetic_algorithm.mutation_probability = float.Parse(mutation_rate.text);
            grid_manager.genetic_algorithm.satisfactory_fitness_level = float.Parse(satisfactory_fitness_level.text);

            if (GameObject.Find("Start") && GameObject.Find("Finish"))
            {
                algorithm_params.SetActive(false);
                algorithm_info.SetActive(true);
                population_size_text.text += grid_manager.genetic_algorithm.population_size;
                chromosome_length_text.text += grid_manager.genetic_algorithm.chromosome_length;
                crossover_rate_text.text += (grid_manager.genetic_algorithm.crossover_probability * 100) + "%";
                mutation_rate_text.text += (grid_manager.genetic_algorithm.mutation_probability * 100) + "%";
                grid_manager.StartGeneticAlgorithm();
            }
            else
            {
                LogText.text += "Установите старт и финиш;\n";
            }
        }
        catch (Exception)
        {
            LogText.text += "Ошибка при указании параметров алгоритма;\n";
        }
    }
}
