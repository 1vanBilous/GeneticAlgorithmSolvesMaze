using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridManager : MonoBehaviour
{
    public GameObject reference_square;
    public Sprite grass_sprite;
    public Sprite selected_grass_sprite;
    public Sprite wall_sprite;
    public Sprite selected_wall_sprite;
    public Sprite start_sprite;
    public Sprite selected_start_sprite;
    public Sprite finish_sprite;
    public Sprite selected_finish_sprite;
    public Sprite path_sprite;

    private Vector2 start_position;
    private Vector2 finish_position;
    private float start_finish_distance;
    public float distance;
    private bool start_ready = false;
    private bool finish_ready = false;

    public int rows;
    public int cols;
    public int tile_size;
    public int[,] grid;

    public GeneticAlgorithm genetic_algorithm;
    public UIManager ui_manager;

    private bool genetic_algorithm_is_paused = false;
    public bool last_step_finish_only = true;

    public int nuber_of_successful_steps;
    public int nuber_of_successful_steps_done;
    public bool order_to_stop = false;
    public bool grid_is_blocked = false;

    void Start()
    {
        grid = new int[rows, cols];
        GenerateGrid();

        genetic_algorithm = new GeneticAlgorithm();
        genetic_algorithm.grid_manager = this;
    }

    void Update()
    {
        if (!grid_is_blocked)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                if (hit.collider != null)
                {
                    string[] indexes = hit.collider.gameObject.name.Split('-');
                    if (hit.collider.gameObject.tag != "Edge")
                    {
                        if (hit.collider.gameObject.tag == "Grass")
                        {
                            hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = selected_wall_sprite;
                            hit.collider.gameObject.tag = "Wall";
                            grid[int.Parse(indexes[0]), int.Parse(indexes[1])] = 1;
                        }
                        else if (hit.collider.gameObject.tag == "Start" || hit.collider.gameObject.tag == "Finish")
                        {
                            if (hit.collider.gameObject.tag == "Start")
                            {
                                start_ready = false;
                            }
                            if (hit.collider.gameObject.tag == "Finish")
                            {
                                finish_ready = false;
                            }
                            hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = selected_wall_sprite;
                            hit.collider.gameObject.tag = "Wall";
                            grid[int.Parse(indexes[0]), int.Parse(indexes[1])] = 1;
                        }
                        else
                        {
                            hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = selected_grass_sprite;
                            hit.collider.gameObject.tag = "Grass";
                            grid[int.Parse(indexes[0]), int.Parse(indexes[1])] = 0;
                        }
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                if (hit.collider != null)
                {
                    string[] indexes = hit.collider.gameObject.name.Split('-');

                    if (hit.collider.gameObject.tag != "Edge")
                    {
                        if (hit.collider.gameObject.tag == "Start")
                        {
                            hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = selected_grass_sprite;
                            hit.collider.gameObject.tag = "Grass";
                            start_ready = false;
                        }
                        else
                        {
                            if (start_ready)
                            {
                                GameObject old_start = GameObject.FindGameObjectWithTag("Start");
                                old_start.GetComponent<SpriteRenderer>().sprite = grass_sprite;
                                old_start.gameObject.tag = "Grass";

                            }
                            if (hit.collider.gameObject.tag == "Finish")
                            {
                                finish_ready = false;
                            }
                            hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = selected_start_sprite;
                            hit.collider.gameObject.tag = "Start";
                            start_position = new Vector2(hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y);
                            start_ready = true;
                        }
                    }
                }
            }

            if (Input.GetMouseButtonDown(2))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                if (hit.collider != null)
                {
                    string[] indexes = hit.collider.gameObject.name.Split('-');

                    if (hit.collider.gameObject.tag != "Edge")
                    {
                        if (hit.collider.gameObject.tag == "Finish")
                        {
                            hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = selected_grass_sprite;
                            hit.collider.gameObject.tag = "Grass";
                            grid[int.Parse(indexes[0]), int.Parse(indexes[1])] = 0;
                            finish_ready = false;
                        }
                        else
                        {
                            if (finish_ready)
                            {
                                GameObject old_finish = GameObject.FindGameObjectWithTag("Finish");
                                old_finish.GetComponent<SpriteRenderer>().sprite = grass_sprite;
                                old_finish.gameObject.tag = "Grass";
                                grid[int.Parse(old_finish.name.Split('-')[0]), int.Parse(old_finish.name.Split('-')[1])] = 0;

                            }
                            if (hit.collider.gameObject.tag == "Start")
                            {
                                start_ready = false;
                            }
                            hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = selected_finish_sprite;
                            hit.collider.gameObject.tag = "Finish";
                            grid[int.Parse(indexes[0]), int.Parse(indexes[1])] = 2;
                            finish_position = new Vector2(hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y);
                            finish_ready = true;
                        }
                    }
                }
            }
        }
        RunGeneticAlgorithm();
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                GameObject tile = Instantiate(reference_square, transform);
                tile.name = i + "-" + j;

                float pos_X = j * tile_size;
                float pos_Y = -(i * tile_size);

                tile.transform.position = new Vector2(pos_X, pos_Y);

                if ((i == 0 || i+1 == rows) || (j == 0 || j + 1 == cols))
                {
                    tile.GetComponent<SpriteRenderer>().sprite = wall_sprite;
                    tile.tag = "Edge";
                    grid[i, j] = 1;
                }
                else
                {
                    grid[i, j] = 0;
                }
            }
        }

        float grid_W = cols * tile_size;
        float grid_H = rows * tile_size;
        transform.position = new Vector2(-grid_W / 2 + tile_size / 2, grid_H / 2 + tile_size / 2);
    }

    public void SetLastStepParam(bool value)
    {
        last_step_finish_only = value;
    }

    public void StartGeneticAlgorithm()
    {
        genetic_algorithm.CreateInitialPopulation();
        genetic_algorithm.generation = 1;
        start_finish_distance = Vector2.Distance(start_position, finish_position);
        genetic_algorithm.busy = true;
        grid_is_blocked = true;
        order_to_stop = false;
    }

    public int[] Move(int[] position, int direction)
    {
        switch (direction)
        {
            case 0: //Up
                if(grid[position[0] + 1, position[1]] != 1)
                {
                    position[0] += 1;
                    nuber_of_successful_steps++;
                }
                break;
            case 1: //Down
                if (grid[position[0] - 1, position[1]] != 1)
                {
                    position[0] -= 1;
                    nuber_of_successful_steps++;
                }
                break;
            case 2: //Left
                if (grid[position[0], position[1] - 1] != 1)
                {
                    position[1] -= 1;
                    nuber_of_successful_steps++;
                }
                break;
            case 3: //Right
                if (grid[position[0], position[1] + 1] != 1)
                {
                    position[1] += 1;
                    nuber_of_successful_steps++;
                }
                break;
        }
        return position;
    }

    public double TestRoute(List<int> directions)
    {
        GameObject start_square = GameObject.FindGameObjectWithTag("Start");
        int[] position = new int[2] { int.Parse(start_square.name.Split('-')[0]), int.Parse(start_square.name.Split('-')[1])};

        foreach (int direction in directions)
        {
            position = Move(position, direction);

            //If passed by finish
            if (!last_step_finish_only)
            {
                if (grid[position[0], position[1]] == 2)
                {
                    return 1;
                }
            }
        }

        GameObject end_of_path = GameObject.Find(position[0] + "-" + position[1]);
        distance = Vector2.Distance(end_of_path.transform.position, finish_position);

        double result = 1 - (distance / start_finish_distance);
        if (result < 0)
        {
            return 0;
        }
        return 1 - (distance / start_finish_distance);
    }

    public void ClearPathTiles()
    {
        GameObject[] steps = GameObject.FindGameObjectsWithTag("Path");
        foreach(GameObject step in steps)
        {
            step.tag = "Grass";
            step.GetComponent<SpriteRenderer>().sprite = grass_sprite;
            grid[int.Parse(step.name.Split('-')[0]), int.Parse(step.name.Split('-')[1])] = 0;
        }
    }

    public void ClearGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if((i == 0 || i + 1 == rows) || (j == 0 || j + 1 == cols))
                {
                    continue;
                }
                if(grid[i, j] == 1)
                {
                    GameObject former_wall = GameObject.Find(i + "-" + j);
                    former_wall.GetComponent<SpriteRenderer>().sprite = grass_sprite;
                    former_wall.tag = "Grass";
                    grid[i, j] = 0;
                }
            }
        }

        if (GameObject.FindGameObjectWithTag("Start"))
        {
            GameObject former = GameObject.FindGameObjectWithTag("Start");;
            former.GetComponent<SpriteRenderer>().sprite = grass_sprite;
            former.tag = "Grass";
            grid[int.Parse(former.name.Split('-')[0]), int.Parse(former.name.Split('-')[1])] = 0;
            start_ready = false;
        }
        if (GameObject.FindGameObjectWithTag("Finish"))
        {
            GameObject former = GameObject.FindGameObjectWithTag("Finish"); ;
            former.GetComponent<SpriteRenderer>().sprite = grass_sprite;
            former.tag = "Grass";
            grid[int.Parse(former.name.Split('-')[0]), int.Parse(former.name.Split('-')[1])] = 0;
            finish_ready = false;
        }

        ClearPathTiles();
    }

    public IEnumerator RenderFittestChromosomePath()
    {
        genetic_algorithm_is_paused = true;
        ClearPathTiles();

        Genome fittest_genome = genetic_algorithm.fittest_genome;
        List<int> directions = genetic_algorithm.Decode(fittest_genome.bits);

        GameObject start_square = GameObject.FindGameObjectWithTag("Start");
        int[] position = new int[2] { int.Parse(start_square.name.Split('-')[0]), int.Parse(start_square.name.Split('-')[1]) };

        nuber_of_successful_steps = 0;

        foreach (int direction in directions)
        {
            position = Move(position, direction);

            //Finish was passed by
            if (!last_step_finish_only)
            {
                if (grid[position[0], position[1]] == 2)
                {
                    break;
                }
            }

            GameObject current_step = GameObject.Find(position[0] + "-" + position[1]);
            if (!current_step.CompareTag("Start") && !current_step.CompareTag("Finish"))
            {
                current_step.tag = "Path";
                current_step.GetComponent<SpriteRenderer>().sprite = path_sprite;
            }
            yield return new WaitForSeconds(0.01f);
        }
        nuber_of_successful_steps_done = nuber_of_successful_steps;

        //Finish was reached
        if (!genetic_algorithm.busy && !order_to_stop)
        {
            ui_manager.Finish();
        }

        genetic_algorithm_is_paused = false;
        if (order_to_stop)
        {
            ClearPathTiles();
        }
    }

    private void RunGeneticAlgorithm()
    {
        if (genetic_algorithm.busy)
        {
            if (!genetic_algorithm_is_paused)
            {
                genetic_algorithm.Epoch();
                StartCoroutine(RenderFittestChromosomePath());
            }
            
        }
    }
}
