using SpaceMath;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class ShipTests
{
    private Ships ship;

    [SetUp]
    public void Setup()
    {
        ship = new Ships();

    }

    [UnityTest]
    public IEnumerator IncreaseHealth()
    {
        int original_health = ship.Health;
        ship.IncreaseHealth(1);

        yield return null;

        Assert.AreEqual(ship.Health, (original_health + 1));
    }

    [UnityTest]
    public IEnumerator DecreaseHealth()
    {

        int original_health = ship.Health;
        ship.ApplyDamage(1);

        yield return null;

        Assert.AreEqual(ship.Health, (original_health - 1));
    }

    [UnityTest]
    public IEnumerator DecreaseEnergy()
    {

        int original_energy = ship.Energy;
        ship.DecreaseEnergy(10);

        yield return null;

        Assert.AreEqual(ship.Energy, (original_energy - 10));
    }

    [UnityTest]
    public IEnumerator IncreaseEnergy()
    {

        int original_energy = ship.Energy;
        ship.IncreaseEnergy(10);

        yield return null;

        Assert.AreEqual(ship.Energy, (original_energy + 10));
    }

    [UnityTest]
    public IEnumerator OutOfEnergy()
    {

        int original_energy = ship.Energy;
        ship.DecreaseEnergy(original_energy);

        yield return null;

        Assert.True(ship.IsShipOutOfEnergy());
        Assert.True(ship.IsShipDead());
    }

    //[UnityTest]
    //public IEnumerator ShieldsOn()
    //{

    //    int originalEnergy = ship.Energy;
    //    ship.OpenShields();        
    //    int energyAfterShields = ship.Energy;        

    //    yield return null;

    //    Assert.Less(energyAfterShields, originalEnergy);

    //}

    //[UnityTest]
    //public IEnumerator ShieldsOff()
    //{

    //    int original_health = ship.Health;
    //    ship.OpenShields();
    //    ship.CloseShields();
    //    int energyBeforeDamage = ship.Energy;
    //    ship.ApplyDamage(1);

    //    yield return null;

    //    Assert.AreEqual(ship.Health, original_health-1);
    //    Assert.AreEqual(ship.Energy, energyBeforeDamage);

    //}

    //[UnityTest]
    //public IEnumerator ShieldsWork()
    //{

    //    int original_health = ship.Health;
    //    ship.OpenShields();
    //    int energyBeforeDamage = ship.Energy;
    //    ship.ApplyDamage(1);

    //    yield return null;

    //    Assert.AreEqual(ship.Health, original_health);
    //    Assert.AreEqual(ship.Energy, energyBeforeDamage - 10);

    //}



    //// A Test behaves as an ordinary method
    //[Test]
    //public void TestSuiteSimplePasses()
    //{
    //    // Use the Assert class to test conditions
    //}

    //// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    //// `yield return null;` to skip a frame.
    //[UnityTest]
    //public IEnumerator TestSuiteWithEnumeratorPasses()
    //{
    //    // Use the Assert class to test conditions.
    //    // Use yield to skip a frame.
    //    yield return null;
    //}


}
