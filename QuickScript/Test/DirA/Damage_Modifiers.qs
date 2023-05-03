Damage_Modifier
{
    *int Modify_damge(int damage);*
}


ExtraDamage *: Damage_Modifier*
{
    float modifier;
}


Armor *: Damage_Modifier*
{
    float protection;
}
