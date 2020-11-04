using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
{
    public List<Genome> genomes;
    public List<Genome> last_generation_genomes;
    public Genome fittest_genome;
    public int generation = 0;

    public int population_size;
    public int chromosome_length;
    private int gene_length = 2;
    public float crossover_probability;
    public float mutation_probability;
    public double satisfactory_fitness_level;

    public double best_fitness_score;
    public double total_fitness_score;

    public bool busy = false;

    public GridManager grid_manager;

    public GeneticAlgorithm()
    {
        genomes = new List<Genome>();
        last_generation_genomes = new List<Genome>();
    }

    public void CreateInitialPopulation()
    {
        genomes.Clear();

        for(int i=0; i < population_size; i++)
        {
            Genome initial_genome = new Genome(chromosome_length);
            genomes.Add(initial_genome);
        }
    }

    public void Crossover(List<int> mom, List<int> dad, List<int> baby1, List<int> baby2)
    {
        if (UnityEngine.Random.value > crossover_probability || mom == dad)
        {
            baby1.AddRange(mom);
            baby2.AddRange(dad);

            return;
        }

        //Define crossover point
        System.Random rnd = new System.Random();
        int crossover_point = rnd.Next(0, chromosome_length - 1);

        for (int i = 0; i < crossover_point; i++)
        {
            baby1.Add(mom[i]);
            baby2.Add(dad[i]);
        }

        for (int i = crossover_point; i < mom.Count; i++)
        {
            baby1.Add(dad[i]);
            baby2.Add(mom[i]);
        }
    }

    public void Mutate(List<int> bits)
    {
        for(int i=0; i<bits.Count; i++)
        {
            if (UnityEngine.Random.value < mutation_probability)
            {
                if (bits[i] == 0) { bits[i] = 1; }
                else { bits[i] = 0; }
            }
        }
    }


    public void UpdateFitnessScores()
    {
        total_fitness_score = 0;
        best_fitness_score = 0;

        for (int i=0; i<population_size; i++)
        {
            List<int> directions = Decode(genomes[i].bits);

            genomes[i].fitness = grid_manager.TestRoute(directions);
            total_fitness_score += genomes[i].fitness;

            if(genomes[i].fitness <= 0)
            {
                best_fitness_score = 0;
                fittest_genome = genomes[i];
            }
            else
            {
                if (genomes[i].fitness > best_fitness_score)
                {
                    best_fitness_score = genomes[i].fitness;
                    fittest_genome = genomes[i];
                    if (fittest_genome.fitness >= satisfactory_fitness_level)
                    {
                        busy = false;
                        return;
                    }
                }
            }
        }
    }

    public Genome RouletteWheelSelection()
    {
        double slice = UnityEngine.Random.value * total_fitness_score;
        double total = 0;
        int selected_genome = 0;

        for (int i = 0; i < population_size; i++)
        {
            total += genomes[i].fitness;

            if(total > slice)
            {
                selected_genome = i;
                break;
            }
        }
        return genomes[selected_genome];
    }

    public void Epoch()
    {
        if (!busy) return;

        UpdateFitnessScores();

        if (!busy) return;

        int number_of_new_babies = 0;
        List<Genome> babies = new List<Genome>();

        while (number_of_new_babies < population_size)
        {
            Genome mom = RouletteWheelSelection();
            Genome dad = RouletteWheelSelection();
            Genome baby1 = new Genome();
            Genome baby2 = new Genome();
            Crossover(mom.bits, dad.bits, baby1.bits, baby2.bits);
            Mutate(baby1.bits);
            Mutate(baby2.bits);
            babies.Add(baby1);
            babies.Add(baby2);

            number_of_new_babies += 2;
        }

        genomes = babies;
        generation++;
    }


    //-----------Convert bits into directions-----------
    //      0 = Up, 1 = Down, 2 = Left, 3 = Right
    //--------------------------------------------------
    public List<int> Decode(List<int> bits)
    {
        List<int> directions = new List<int>();

        for (int gene_index = 0; gene_index < bits.Count; gene_index += gene_length)
        {
            List<int> gene = new List<int>();

            for (int bit_index = 0; bit_index < gene_length; bit_index++)
            {
                gene.Add(bits[gene_index + bit_index]);
            }

            directions.Add(GeneToInt(gene));
        }

        return directions;
    }

    //Convert list of bits into integers
    public int GeneToInt(List<int> gene)
    {
        string gene_str = "";
        for(int i=0; i<gene.Count; i++)
        {
            gene_str += gene[i];
        }
        return Convert.ToInt32(gene_str, 2);
    }
}
