namespace GrpcContracts
{
    public class Class1
    {
	    private CustomerProto customer = new CustomerProto();

	    public void Foo()
	    {
		    if (customer.Country == null)
			    throw new Exception();
	    }
    }
}
