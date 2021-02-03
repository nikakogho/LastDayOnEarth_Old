using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemBlueprintStuff;

public static class InsideMethods
{
    public static int ToInt(this Item.MaxInOneSlotEnum max)
    {
        return (int)max;
    }

    public static List<ItemBlueprint> ToItemBlueprintList(this List<ChanceItemBlueprint> list, int minTotal, int maxTotal)
    {
        var answer = new List<ItemBlueprint>();

        float total = 0;
        float sum = 0;

        int amount = Random.Range(minTotal, maxTotal + 1);

        foreach(var item in list)
        {
            total += item.chance;
        }

        for (int i = 0; i < amount; i++)
        {
            float rand = Random.Range(0, total);

            ItemBlueprint chosen = null;

            foreach (var item in list)
            {
                sum += item.chance;

                if (sum >= rand)
                {
                    chosen = item.blueprint;
                    break;
                }
            }

            if (chosen != null)
            {
                answer.Add(chosen);
            }
        }

        return answer;
    }

    public static List<ItemBlueprint> ToItemBlueprintList(this List<MinMaxItemBlueprint> list)
    {
        var answer = new List<ItemBlueprint>();

        foreach(var item in list)
        {
            int amount = Random.Range(item.minAmount, item.maxAmount + 1);
            var blueprint = new ItemBlueprint(item.item, amount);

            answer.Add(blueprint);
        }

        return answer;
    }

    public static List<ItemBlueprint> ToItemBlueprintList(this List<ChanceMinMaxBlueprint> list)
    {
        var answer = new List<ItemBlueprint>();

        foreach(var item in list)
        {
            float rand = Random.Range(0f, 1f);

            if(rand <= item.chance)
            {
                int amount = Lerp(item.blueprint.minAmount, item.blueprint.maxAmount, 1f - rand / item.chance);
                
                answer.Add(new ItemBlueprint(item.blueprint.item, amount));
            }
        }

        return answer;
    }

    public static int Lerp(int a, int b, float value)
    {
        return a + (int)((b - a) * value);
    }

    public static bool Contains<T>(this T[] array, T item)
    {
        foreach(T element in array)
        {
            if (element.Equals(item)) return true;
        }

        return false;
    }

    public static Vector3 DirFromAngle(this float angle)
    {
        angle *= Mathf.Deg2Rad;

        return new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
    }
}
