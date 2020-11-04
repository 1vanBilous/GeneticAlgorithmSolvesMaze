using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject algorithm_params_panel;
    public GameObject algorithm_info_panel;
    public GameObject log_panel;
    public GameObject stop_panel;
    public GameObject confirm_results_panel;
    public GameObject clear_grid_panel;

    public InputField population_size_input;
    public InputField chromosome_length_input;
    public InputField crossover_probability_input;
    public InputField mutation_probability_input;
    public InputField satisfactory_fitness_level_input;
    public Toggle last_step_finish_only_toggle;

    public Text LogText;

    public Text generation_text;
    public Text population_size_text;
    public Text chromosome_length_text;
    public Text crossover_rate_text;
    public Text mutation_rate_text;
    public Text available_steps_text;
    public Text steps_done_text;
    public Text distance_to_finish_text;
    public Text fitness_text;

    public GridManager grid_manager;

    private void Update()
    {
        generation_text.text = grid_manager.genetic_algorithm.generation.ToString();
        distance_to_finish_text.text = grid_manager.distance.ToString();
        fitness_text.text = grid_manager.genetic_algorithm.best_fitness_score.ToString();
        steps_done_text.text = grid_manager.nuber_of_successful_steps_done.ToString();
    }

    public void DoClearGrid()
    {
        grid_manager.ClearGrid();
    }

    public void CheckInputsAndStartGeneticAlgorithm()
    {
        if (CheckInputs())
        {
            grid_manager.genetic_algorithm.population_size = int.Parse(population_size_input.text);
            grid_manager.genetic_algorithm.chromosome_length = int.Parse(chromosome_length_input.text);
            grid_manager.genetic_algorithm.crossover_probability = float.Parse(crossover_probability_input.text);
            grid_manager.genetic_algorithm.mutation_probability = float.Parse(mutation_probability_input.text);
            grid_manager.genetic_algorithm.satisfactory_fitness_level = float.Parse(satisfactory_fitness_level_input.text);
            grid_manager.last_step_finish_only = last_step_finish_only_toggle.GetComponent<Toggle>().isOn;

            if (GameObject.FindGameObjectWithTag("Start") && GameObject.FindGameObjectWithTag("Finish"))
            {
                algorithm_params_panel.SetActive(false);
                algorithm_info_panel.SetActive(true);
                log_panel.SetActive(false);
                stop_panel.SetActive(true);
                clear_grid_panel.SetActive(false);
                population_size_text.text = grid_manager.genetic_algorithm.population_size.ToString();
                chromosome_length_text.text = grid_manager.genetic_algorithm.chromosome_length.ToString();
                crossover_rate_text.text = (grid_manager.genetic_algorithm.crossover_probability * 100) + "%";
                mutation_rate_text.text = (grid_manager.genetic_algorithm.mutation_probability * 100) + "%";
                available_steps_text.text = (grid_manager.genetic_algorithm.chromosome_length / 2).ToString();
                LogText.text = "";
                grid_manager.StartGeneticAlgorithm();
            }
            else
            {
                LogText.text += "Select start and finish;\n";
            }
        }
    }

    private bool CheckInputs()
    {
        if(population_size_input.text.Length == 0)
        {
            LogText.text += "Enter population size;\n";
            return false;
        }

        if(chromosome_length_input.text.Length == 0)
        {
            LogText.text += "Enter chromosome length;\n";
            return false;
        }
        if (int.Parse(chromosome_length_input.text) % 2 != 0)
        {
            LogText.text += "Chromosome length must divide by 2 witout a remainder;\n";
            return false;
        }

        if (crossover_probability_input.text.Length == 0)
        {
            LogText.text += "Enter crossover probability;\n";
            return false;
        }
        if(float.Parse(crossover_probability_input.text) > 1 || float.Parse(crossover_probability_input.text) < 0)
        {
            LogText.text += "Crossover probability must be within [0;1];\n";
            return false;
        }

        if(mutation_probability_input.text.Length == 0)
        {
            LogText.text += "Enter mutation probability;\n";
            return false;
        }
        if(float.Parse(mutation_probability_input.text) > 1 || float.Parse(mutation_probability_input.text) < 0)
        {
            LogText.text += "Mutation probability must be within [0;1];\n";
            return false;
        }

        if(satisfactory_fitness_level_input.text.Length == 0)
        {
            LogText.text += "Enter satisfactory value for fitness function;\n";
            return false;
        }
        if(float.Parse(satisfactory_fitness_level_input.text) > 1 || float.Parse(satisfactory_fitness_level_input.text) < 0)
        {
            LogText.text += "Satisfactory value for fitness function must be within [0;1];\n";
            return false;
        }

        return true;
    }

    public void DoStopGeneticAlgorithm()
    {
        algorithm_params_panel.SetActive(true);
        algorithm_info_panel.SetActive(false);
        log_panel.SetActive(true);
        stop_panel.SetActive(false);
        clear_grid_panel.SetActive(true);
        grid_manager.genetic_algorithm.busy = false;
        grid_manager.order_to_stop = true;
        grid_manager.grid_is_blocked = false;
    }

    public void Finish()
    {
        confirm_results_panel.SetActive(true);
        log_panel.SetActive(true);
        stop_panel.SetActive(false);
        LogText.text += "Task completed. Maze has been solved;\n";
    }

    public void DoConfirmResults()
    {
        confirm_results_panel.SetActive(false);
        algorithm_params_panel.SetActive(true);
        algorithm_info_panel.SetActive(false);
        clear_grid_panel.SetActive(true);
        LogText.text = "";
        grid_manager.grid_is_blocked = false;

        grid_manager.ClearPathTiles();
    }
}
