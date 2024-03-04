using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public PlayerController playerController;
    public StackController stackController;
    private float timer;


    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponentInParent<Machine>() && !other.GetComponent<MoneyStack>())
        {
            var machine = other.transform.GetComponentInParent<Machine>();
            if (machine.machineState == MachineState.Open)
            {
                if (machine as WorkoutMachine)
                {
                    if ((machine as WorkoutMachine).workoutMachineState == WorkoutMachineState.Busy)
                    {
                        stackController.CheckIfStackHasItem((machine as WorkoutMachine).machineCustomer);
                    }

                    if (other.GetComponent<MachineUpgradeArea>())
                    {
                        other.GetComponentInParent<MachineUpgradeController>().SetUpgradeValues();
                        other.GetComponent<MachineUpgradeArea>().PlayerOnArea();
                    }
                }
            }
        }

        if (other.transform.GetComponent<Customer>())
        {
            var customer = other.transform.GetComponent<Customer>();
            if (customer.customerMachine as Pool)
            {
                stackController.CheckIfStackHasItem(customer);
            }
        }

        if (other.GetComponent<MachineUpgradeArea>())
        {
            other.GetComponent<MachineUpgradeArea>().PlayerOnArea();
        }

        if (other.GetComponent<MoneyStack>())
        {
            playerController.money += other.GetComponent<MoneyStack>().PlayerCollectedAllMoney(transform.position);
            EventManager.GetGameData().totalMoneyAmount = playerController.money;
            EventManager.MoneyUpdated();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MachineUpgradeArea>())
        {
            other.GetComponent<MachineUpgradeArea>().PlayerOffArea();
        }

        if (other.transform.GetComponentInParent<WaterMachine>() && other.GetComponent<MachineInteraction>())
        {
            playerController.StackIsMax(false);
        }

        if (other.transform.GetComponentInParent<TowelMachine>() && other.GetComponent<MachineInteraction>())
        {
            playerController.StackIsMax(false);
        }

        if (other.transform.GetComponentInParent<SnackMachine>() && other.GetComponent<MachineInteraction>())
        {
            playerController.StackIsMax(false);
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("MachineBuyArea"))
        {
            var machine = other.transform.GetComponentInParent<Machine>();
            if (playerController.money >= machine.price)
            {
                if (machine.machineState == MachineState.Close)
                {

                    if (machine.FillUITime(Time.deltaTime))
                    {
                        machine.MachinePurchased();
                        playerController.money -= (int)machine.price;
                        EventManager.GetGameData().totalMoneyAmount = playerController.money;
                        EventManager.MoneyUpdated();
                    }
                }
            }
        }

        if (other.transform.GetComponentInParent<WaterMachine>() && other.GetComponent<MachineInteraction>())
        {
            var machine = other.transform.GetComponentInParent<WaterMachine>();
            if (playerController.money >= machine.waterPrice)
            {
                if (machine.machineState == MachineState.Open)
                {
                    if (stackController.CheckIfCanBuyStackable(CustomerNeed.Water))
                    {
                        timer += Time.deltaTime * 2;
                        if (machine.FillBuyUI(Time.deltaTime))
                        {
                            machine.machineUI.workoutTimeImage.fillAmount = 0;
                            timer = 0;
                            stackController.StackObject(machine.SpawnWaterBottle());
                            playerController.money -= machine.waterPrice;

                            EventManager.GetGameData().totalMoneyAmount = playerController.money;
                            EventManager.MoneyUpdated();
                        }
                    }
                    else
                    {
                        playerController.StackIsMax(true);
                    }
                }
            }
        }

        if (other.transform.GetComponentInParent<TowelMachine>() && other.GetComponent<MachineInteraction>())
        {
            var machine = other.transform.GetComponentInParent<TowelMachine>();
            if (playerController.money >= machine.towelPrice)
            {
                if (machine.machineState == MachineState.Open)
                {
                    if (stackController.CheckIfCanBuyStackable(CustomerNeed.Towel))
                    {
                        timer += Time.deltaTime * 2;
                        if (machine.FillBuyUI(Time.deltaTime))
                        {
                            machine.machineUI.workoutTimeImage.fillAmount = 0;
                            timer = 0;
                            stackController.StackObject(machine.SpawnTower());
                            playerController.money -= machine.towelPrice;

                            EventManager.GetGameData().totalMoneyAmount = playerController.money;
                            EventManager.MoneyUpdated();
                        }
                    }
                    else
                    {
                        playerController.StackIsMax(true);
                    }
                }
            }
        }

        if (other.transform.GetComponentInParent<SnackMachine>() && other.GetComponent<MachineInteraction>())
        {
            var machine = other.transform.GetComponentInParent<SnackMachine>();
            if (playerController.money >= machine.snackPrice)
            {
                if (machine.machineState == MachineState.Open)
                {
                    if (stackController.CheckIfCanBuyStackable(CustomerNeed.Snack))
                    {
                        timer += Time.deltaTime * 2;
                        if (machine.FillBuyUI(timer))
                        {
                            machine.machineUI.workoutTimeImage.fillAmount = 0;
                            timer = 0;
                            stackController.StackObject(machine.SpawnSnack());
                            playerController.money -= machine.snackPrice;

                            EventManager.GetGameData().totalMoneyAmount = playerController.money;
                            EventManager.MoneyUpdated();
                        }
                        else
                        {
                            playerController.StackIsMax(true);
                        }
                    }
                }
            }
        }
    }
}