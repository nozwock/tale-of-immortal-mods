namespace TaleOfImmortalCheat.UI.Panels;

public interface Field
{
	int TabIndex { get; set; }

	void Save(AttributesState state, int? tabIndex);

	void Reset(AttributesState state);
}
