using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Samurai karakter vagasat es uteset megvalosito osztaly
public class SamuraiAttack : MonoBehaviour
{
    // Referencia a jutalom kezelo objektumra
    public RewardManager rewardManager;
    // Animator referencia a karakter tamadasaihoz
    private Animator animator;
    // kard collider, ezzel erzekeljuk, ha egy objektum ami benne van az animacio altal lefedett teruleten (ha vaghato, elvagjuk)
    private BoxCollider2D swordCollider;
    // Referencia a karakter mozgasaert felelos osztalyra, rajta keresztul erjuk el, hogy aktualisan a karakter melyik iranyba nez
    // erre a kard collider-enek megfelelo iranyba tartasa miatt van szukseg, es hogy jo iranyba repuljenek a szetvagott darabok
    private SamuraiMovement samuraiMovement;

    // Inicializalja az animatorat a karakternek, a kard collider-et, es a karakter mozgasaert felelos osztaly objektum peldanyat
    private void Awake() {
        animator = GetComponent<Animator>();
        swordCollider = transform.Find("Sword").GetComponent<BoxCollider2D>();
        samuraiMovement = GetComponent<SamuraiMovement>();
    }

    private void Update() {
        CheckForAttack(); // ellenorizzuk, hogy tortent-e tamadasi kiserlet a felhasznalotol
    }

    private void CheckForAttack() {
        // 1.fajta utesi tipus, a Q billenytu lenyomasara
        if (Input.GetKeyDown("q")) {
            animator.SetInteger("AttackType", 0);
            animator.SetTrigger("AttackTriggered"); // lejatszuk az animaciot

            // Elkerjuk az osszes karddal torteno collision-t (a Sliceable layeren levo objektumokra szurve)
            Collider2D[] colliders = GetCollisionsWithSliceableOrHitableObjects();
            // ha volt talalat a Sliceable layer-en akkor a vagas/utes elindul a TriggerSlicing()-al
            if (GetCollisionsWithSliceableOrHitableObjects().Length > 0) {
                // konstans kard sebesseg, ami hatassal lesz a megutott objektum elmozditasakor
                // a 'q' billentyuhoz a tamadas animacio lentrol-elore-felfele suhint
                Vector2 swordVelo = new Vector2(samuraiMovement.GetCharacterDir().x * 3, 2);

                TriggerSlicingOrHitting(colliders, swordVelo);
            }

            // 2.fajta utesi tipus, a Q billenytu lenyomasara
        } else if (Input.GetKeyDown("e")) {
            animator.SetInteger("AttackType", 1);
            animator.SetTrigger("AttackTriggered");  // lejatszuk az animaciot

            Collider2D[] colliders = GetCollisionsWithSliceableOrHitableObjects();
            // ha volt talalat a Sliceable layer-en akkor a vagas/utes elindul a TriggerSlicing()-al
            if (GetCollisionsWithSliceableOrHitableObjects().Length > 0) {
                // konstans kard sebesseg, ami hatassal lesz a megutott objektum elmozditasakor
                // az 'e'billentyuhoz a tamadas animacio elore fele suhint
                Vector2 swordVelo = new Vector2(samuraiMovement.GetCharacterDir().x * 3, 0);
                                                                                            
                TriggerSlicingOrHitting(colliders, swordVelo);
            }
        }
    }

    private Collider2D[] GetCollisionsWithSliceableOrHitableObjects() {
        // a kard collider-enek kozeppontjara a collision detekcional van szukseg az OverlapBoxALl() metodus meghivasanak
        Vector3 centerOfSwordCollider = swordCollider.transform.position   // a kard collider-enek pozicioja es innen szamitjuk az offsetet a kozepponthoz
                                        + new Vector3(samuraiMovement.GetCharacterDir().x  // helyes iranyba forgatjuk a kard collider-et
                                        * swordCollider.offset.x, swordCollider.offset.y, 0); // Az offset-ekbol megkapjuk a kard collider-nek kozeppontjat
        // az editorban minden egyes gameobject, amit a karddal eltudunk vagni vagy megtudunk utni a Sliceable layer-en van
        Collider2D[] colliders = Physics2D.OverlapBoxAll(centerOfSwordCollider, swordCollider.size, 0f, LayerMask.GetMask("Sliceable")); 
        return colliders; // ha volt talalat a Sliceable layer-en akkor true
            
    }

    // akkor hivodik meg, ha volt talalat a Sliceable layer-en, es ekkor ha szetvaghato az objektum szetvagja, ha pedig nem akkor csak meguti
    private void TriggerSlicingOrHitting(Collider2D[] colliders, Vector2 swordVelo) {
        foreach (Collider2D collider in colliders) {
            Debug.Log(collider);
        }
        foreach (Collider2D collider in colliders) { // vegigiteralunk minden vaghato/utheto objektumon
            Sliceable sliceable = collider.gameObject.GetComponent<Sliceable>(); // Elkerjuk az adott objektumhoz tartozo Sliceable osztaly peldanyt
            sliceable.Slice(swordVelo); // meghivjuk rajta a Slice() metodust, amely vegrehajtja a vagast/utest az objektumon, figyelembe veve a kard sebesseget
            if (collider.CompareTag("Reward")) {
                rewardManager.watermelonCount++; // Ha dinnyet vagtunk kette, akkor noveljuk a score szamlalot (csak az egeszben levo dinnyek ernek pontot a belsobb darabjaik mar nem)
            }
        }
    }
}
