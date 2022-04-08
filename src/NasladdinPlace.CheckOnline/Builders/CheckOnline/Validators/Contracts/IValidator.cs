namespace NasladdinPlace.CheckOnline.Builders.CheckOnline.Validators.Contracts
{
    public interface IValidator<in T>
    {
        string Validate(T model);
    }
}