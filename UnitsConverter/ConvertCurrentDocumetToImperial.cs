using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace UnitsConverter
{

    [Transaction(TransactionMode.Manual)]

    class ConvertCurrentDocumetToImperial : IExternalCommand
    {
        //static AddInId appId = new AddInId(new Guid("2554BB9D-7D57-4FB2-B706-DEE97AED7A29"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {

            // the code get the document
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //UIDocument uidoc = this.ActiveUIDocument;
            //Document doc = uidoc.Document;

            //get the units in the document
            Units units = doc.GetUnits();

            //UTLength
            FormatOptions foUTLength = units.GetFormatOptions(UnitType.UT_Length);
            foUTLength.Accuracy = 0.00260416666666667;
            foUTLength.DisplayUnits = DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Length, foUTLength);
            //UTArea
            FormatOptions foUTArea = units.GetFormatOptions(UnitType.UT_Area);
            foUTArea.Accuracy = 0.01;
            foUTArea.DisplayUnits = DisplayUnitType.DUT_SQUARE_FEET;
            units.SetFormatOptions(UnitType.UT_Area, foUTArea);
            //UTVolume
            FormatOptions foUTVolume = units.GetFormatOptions(UnitType.UT_Volume);
            foUTVolume.Accuracy = 0.01;
            foUTVolume.DisplayUnits = DisplayUnitType.DUT_CUBIC_FEET;
            units.SetFormatOptions(UnitType.UT_Volume, foUTVolume);
            //UTAngle
            FormatOptions foUTAngle = units.GetFormatOptions(UnitType.UT_Angle);
            foUTAngle.Accuracy = 0.01;
            foUTAngle.DisplayUnits = DisplayUnitType.DUT_DECIMAL_DEGREES;
            units.SetFormatOptions(UnitType.UT_Angle, foUTAngle);
            //UTHVACDensity
            FormatOptions foUTHVACDensity = units.GetFormatOptions(UnitType.UT_HVAC_Density);
            foUTHVACDensity.Accuracy = 0.0001;
            foUTHVACDensity.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS_PER_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Density, foUTHVACDensity);
            //UTHVACEnergy
            FormatOptions foUTHVACEnergy = units.GetFormatOptions(UnitType.UT_HVAC_Energy);
            foUTHVACEnergy.Accuracy = 1;
            foUTHVACEnergy.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS;
            units.SetFormatOptions(UnitType.UT_HVAC_Energy, foUTHVACEnergy);
            //UTHVACFriction
            FormatOptions foUTHVACFriction = units.GetFormatOptions(UnitType.UT_HVAC_Friction);
            foUTHVACFriction.Accuracy = 0.01;
            foUTHVACFriction.DisplayUnits = DisplayUnitType.DUT_INCHES_OF_WATER_PER_100FT;
            units.SetFormatOptions(UnitType.UT_HVAC_Friction, foUTHVACFriction);
            //UTHVACPower
            FormatOptions foUTHVACPower = units.GetFormatOptions(UnitType.UT_HVAC_Power);
            foUTHVACPower.Accuracy = 1;
            foUTHVACPower.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR;
            units.SetFormatOptions(UnitType.UT_HVAC_Power, foUTHVACPower);
            //UTHVACPowerDensity
            FormatOptions foUTHVACPowerDensity = units.GetFormatOptions(UnitType.UT_HVAC_Power_Density);
            foUTHVACPowerDensity.Accuracy = 0.01;
            foUTHVACPowerDensity.DisplayUnits = DisplayUnitType.DUT_WATTS_PER_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Power_Density, foUTHVACPowerDensity);
            //UTHVACPressure
            FormatOptions foUTHVACPressure = units.GetFormatOptions(UnitType.UT_HVAC_Pressure);
            foUTHVACPressure.Accuracy = 0.01;
            foUTHVACPressure.DisplayUnits = DisplayUnitType.DUT_INCHES_OF_WATER;
            units.SetFormatOptions(UnitType.UT_HVAC_Pressure, foUTHVACPressure);
            //UTHVACTemperature
            FormatOptions foUTHVACTemperature = units.GetFormatOptions(UnitType.UT_HVAC_Temperature);
            foUTHVACTemperature.Accuracy = 1;
            foUTHVACTemperature.DisplayUnits = DisplayUnitType.DUT_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_HVAC_Temperature, foUTHVACTemperature);
            //UTHVACVelocity
            FormatOptions foUTHVACVelocity = units.GetFormatOptions(UnitType.UT_HVAC_Velocity);
            foUTHVACVelocity.Accuracy = 1;
            foUTHVACVelocity.DisplayUnits = DisplayUnitType.DUT_FEET_PER_MINUTE;
            units.SetFormatOptions(UnitType.UT_HVAC_Velocity, foUTHVACVelocity);
            //UTHVACAirflow
            FormatOptions foUTHVACAirflow = units.GetFormatOptions(UnitType.UT_HVAC_Airflow);
            foUTHVACAirflow.Accuracy = 1;
            foUTHVACAirflow.DisplayUnits = DisplayUnitType.DUT_CUBIC_FEET_PER_MINUTE;
            units.SetFormatOptions(UnitType.UT_HVAC_Airflow, foUTHVACAirflow);
            //UTHVACDuctSize
            FormatOptions foUTHVACDuctSize = units.GetFormatOptions(UnitType.UT_HVAC_DuctSize);
            foUTHVACDuctSize.Accuracy = 1;
            foUTHVACDuctSize.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_HVAC_DuctSize, foUTHVACDuctSize);
            //UTHVACCrossSection
            FormatOptions foUTHVACCrossSection = units.GetFormatOptions(UnitType.UT_HVAC_CrossSection);
            foUTHVACCrossSection.Accuracy = 0.01;
            foUTHVACCrossSection.DisplayUnits = DisplayUnitType.DUT_SQUARE_INCHES;
            units.SetFormatOptions(UnitType.UT_HVAC_CrossSection, foUTHVACCrossSection);
            //UTHVACHeatGain
            FormatOptions foUTHVACHeatGain = units.GetFormatOptions(UnitType.UT_HVAC_HeatGain);
            foUTHVACHeatGain.Accuracy = 0.1;
            foUTHVACHeatGain.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR;
            units.SetFormatOptions(UnitType.UT_HVAC_HeatGain, foUTHVACHeatGain);
            //UTElectricalCurrent
            FormatOptions foUTElectricalCurrent = units.GetFormatOptions(UnitType.UT_Electrical_Current);
            foUTElectricalCurrent.Accuracy = 1;
            foUTElectricalCurrent.DisplayUnits = DisplayUnitType.DUT_AMPERES;
            units.SetFormatOptions(UnitType.UT_Electrical_Current, foUTElectricalCurrent);
            //UTElectricalPotential
            FormatOptions foUTElectricalPotential = units.GetFormatOptions(UnitType.UT_Electrical_Potential);
            foUTElectricalPotential.Accuracy = 1;
            foUTElectricalPotential.DisplayUnits = DisplayUnitType.DUT_VOLTS;
            units.SetFormatOptions(UnitType.UT_Electrical_Potential, foUTElectricalPotential);
            //UTElectricalFrequency
            FormatOptions foUTElectricalFrequency = units.GetFormatOptions(UnitType.UT_Electrical_Frequency);
            foUTElectricalFrequency.Accuracy = 1;
            foUTElectricalFrequency.DisplayUnits = DisplayUnitType.DUT_HERTZ;
            units.SetFormatOptions(UnitType.UT_Electrical_Frequency, foUTElectricalFrequency);
            //UTElectricalIlluminance
            FormatOptions foUTElectricalIlluminance = units.GetFormatOptions(UnitType.UT_Electrical_Illuminance);
            foUTElectricalIlluminance.Accuracy = 1;
            foUTElectricalIlluminance.DisplayUnits = DisplayUnitType.DUT_LUX;
            units.SetFormatOptions(UnitType.UT_Electrical_Illuminance, foUTElectricalIlluminance);
            //UTElectricalLuminance
            FormatOptions foUTElectricalLuminance = units.GetFormatOptions(UnitType.UT_Electrical_Luminance);
            foUTElectricalLuminance.Accuracy = 1;
            foUTElectricalLuminance.DisplayUnits = DisplayUnitType.DUT_CANDELAS_PER_SQUARE_METER;
            units.SetFormatOptions(UnitType.UT_Electrical_Luminance, foUTElectricalLuminance);
            //UTElectricalLuminousFlux
            FormatOptions foUTElectricalLuminousFlux = units.GetFormatOptions(UnitType.UT_Electrical_Luminous_Flux);
            foUTElectricalLuminousFlux.Accuracy = 1;
            foUTElectricalLuminousFlux.DisplayUnits = DisplayUnitType.DUT_LUMENS;
            units.SetFormatOptions(UnitType.UT_Electrical_Luminous_Flux, foUTElectricalLuminousFlux);
            //UTElectricalLuminousIntensity
            FormatOptions foUTElectricalLuminousIntensity = units.GetFormatOptions(UnitType.UT_Electrical_Luminous_Intensity);
            foUTElectricalLuminousIntensity.Accuracy = 1;
            foUTElectricalLuminousIntensity.DisplayUnits = DisplayUnitType.DUT_CANDELAS;
            units.SetFormatOptions(UnitType.UT_Electrical_Luminous_Intensity, foUTElectricalLuminousIntensity);
            //UTElectricalEfficacy
            FormatOptions foUTElectricalEfficacy = units.GetFormatOptions(UnitType.UT_Electrical_Efficacy);
            foUTElectricalEfficacy.Accuracy = 1;
            foUTElectricalEfficacy.DisplayUnits = DisplayUnitType.DUT_LUMENS_PER_WATT;
            units.SetFormatOptions(UnitType.UT_Electrical_Efficacy, foUTElectricalEfficacy);
            //UTElectricalWattage
            FormatOptions foUTElectricalWattage = units.GetFormatOptions(UnitType.UT_Electrical_Wattage);
            foUTElectricalWattage.Accuracy = 1;
            foUTElectricalWattage.DisplayUnits = DisplayUnitType.DUT_WATTS;
            units.SetFormatOptions(UnitType.UT_Electrical_Wattage, foUTElectricalWattage);
            //UTColorTemperature
            FormatOptions foUTColorTemperature = units.GetFormatOptions(UnitType.UT_Color_Temperature);
            foUTColorTemperature.Accuracy = 1;
            foUTColorTemperature.DisplayUnits = DisplayUnitType.DUT_KELVIN;
            units.SetFormatOptions(UnitType.UT_Color_Temperature, foUTColorTemperature);
            //UTElectricalPower
            FormatOptions foUTElectricalPower = units.GetFormatOptions(UnitType.UT_Electrical_Power);
            foUTElectricalPower.Accuracy = 1;
            foUTElectricalPower.DisplayUnits = DisplayUnitType.DUT_WATTS;
            units.SetFormatOptions(UnitType.UT_Electrical_Power, foUTElectricalPower);
            //UTHVACRoughness
            FormatOptions foUTHVACRoughness = units.GetFormatOptions(UnitType.UT_HVAC_Roughness);
            foUTHVACRoughness.Accuracy = 0.0001;
            foUTHVACRoughness.DisplayUnits = DisplayUnitType.DUT_DECIMAL_FEET;
            units.SetFormatOptions(UnitType.UT_HVAC_Roughness, foUTHVACRoughness);
            //UTElectricalApparentPower
            FormatOptions foUTElectricalApparentPower = units.GetFormatOptions(UnitType.UT_Electrical_Apparent_Power);
            foUTElectricalApparentPower.Accuracy = 1;
            foUTElectricalApparentPower.DisplayUnits = DisplayUnitType.DUT_VOLT_AMPERES;
            units.SetFormatOptions(UnitType.UT_Electrical_Apparent_Power, foUTElectricalApparentPower);
            //UTElectricalPowerDensity
            FormatOptions foUTElectricalPowerDensity = units.GetFormatOptions(UnitType.UT_Electrical_Power_Density);
            foUTElectricalPowerDensity.Accuracy = 0.01;
            foUTElectricalPowerDensity.DisplayUnits = DisplayUnitType.DUT_WATTS_PER_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_Electrical_Power_Density, foUTElectricalPowerDensity);
            //UTPipingDensity
            FormatOptions foUTPipingDensity = units.GetFormatOptions(UnitType.UT_Piping_Density);
            foUTPipingDensity.Accuracy = 0.0001;
            foUTPipingDensity.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS_PER_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_Piping_Density, foUTPipingDensity);
            //UTPipingFlow
            FormatOptions foUTPipingFlow = units.GetFormatOptions(UnitType.UT_Piping_Flow);
            foUTPipingFlow.Accuracy = 1;
            foUTPipingFlow.DisplayUnits = DisplayUnitType.DUT_GALLONS_US_PER_MINUTE;
            units.SetFormatOptions(UnitType.UT_Piping_Flow, foUTPipingFlow);
            //UTPipingFriction
            FormatOptions foUTPipingFriction = units.GetFormatOptions(UnitType.UT_Piping_Friction);
            foUTPipingFriction.Accuracy = 0.01;
            foUTPipingFriction.DisplayUnits = DisplayUnitType.DUT_FEET_OF_WATER_PER_100FT;
            units.SetFormatOptions(UnitType.UT_Piping_Friction, foUTPipingFriction);
            //UTPipingPressure
            FormatOptions foUTPipingPressure = units.GetFormatOptions(UnitType.UT_Piping_Pressure);
            foUTPipingPressure.Accuracy = 0.01;
            foUTPipingPressure.DisplayUnits = DisplayUnitType.DUT_POUNDS_FORCE_PER_SQUARE_INCH;
            units.SetFormatOptions(UnitType.UT_Piping_Pressure, foUTPipingPressure);
            //UTPipingTemperature
            FormatOptions foUTPipingTemperature = units.GetFormatOptions(UnitType.UT_Piping_Temperature);
            foUTPipingTemperature.Accuracy = 1;
            foUTPipingTemperature.DisplayUnits = DisplayUnitType.DUT_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_Piping_Temperature, foUTPipingTemperature);
            //UTPipingVelocity
            FormatOptions foUTPipingVelocity = units.GetFormatOptions(UnitType.UT_Piping_Velocity);
            foUTPipingVelocity.Accuracy = 1;
            foUTPipingVelocity.DisplayUnits = DisplayUnitType.DUT_FEET_PER_SECOND;
            units.SetFormatOptions(UnitType.UT_Piping_Velocity, foUTPipingVelocity);
            //UTPipingViscosity
            FormatOptions foUTPipingViscosity = units.GetFormatOptions(UnitType.UT_Piping_Viscosity);
            foUTPipingViscosity.Accuracy = 0.01;
            foUTPipingViscosity.DisplayUnits = DisplayUnitType.DUT_CENTIPOISES;
            units.SetFormatOptions(UnitType.UT_Piping_Viscosity, foUTPipingViscosity);
            //UTPipeSize
            FormatOptions foUTPipeSize = units.GetFormatOptions(UnitType.UT_PipeSize);
            foUTPipeSize.Accuracy = 1;
            foUTPipeSize.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_PipeSize, foUTPipeSize);
            //UTPipingRoughness
            FormatOptions foUTPipingRoughness = units.GetFormatOptions(UnitType.UT_Piping_Roughness);
            foUTPipingRoughness.Accuracy = 1E-05;
            foUTPipingRoughness.DisplayUnits = DisplayUnitType.DUT_DECIMAL_FEET;
            units.SetFormatOptions(UnitType.UT_Piping_Roughness, foUTPipingRoughness);
            //UTPipingVolume
            FormatOptions foUTPipingVolume = units.GetFormatOptions(UnitType.UT_Piping_Volume);
            foUTPipingVolume.Accuracy = 0.1;
            foUTPipingVolume.DisplayUnits = DisplayUnitType.DUT_GALLONS_US;
            units.SetFormatOptions(UnitType.UT_Piping_Volume, foUTPipingVolume);
            //UTHVACViscosity
            FormatOptions foUTHVACViscosity = units.GetFormatOptions(UnitType.UT_HVAC_Viscosity);
            foUTHVACViscosity.Accuracy = 0.01;
            foUTHVACViscosity.DisplayUnits = DisplayUnitType.DUT_CENTIPOISES;
            units.SetFormatOptions(UnitType.UT_HVAC_Viscosity, foUTHVACViscosity);
            //UTHVACCoefficientOfHeatTransfer
            FormatOptions foUTHVACCoefficientOfHeatTransfer = units.GetFormatOptions(UnitType.UT_HVAC_CoefficientOfHeatTransfer);
            foUTHVACCoefficientOfHeatTransfer.Accuracy = 0.0001;
            foUTHVACCoefficientOfHeatTransfer.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_SQUARE_FOOT_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_HVAC_CoefficientOfHeatTransfer, foUTHVACCoefficientOfHeatTransfer);
            //UTHVACThermalResistance
            FormatOptions foUTHVACThermalResistance = units.GetFormatOptions(UnitType.UT_HVAC_ThermalResistance);
            foUTHVACThermalResistance.Accuracy = 0.0001;
            foUTHVACThermalResistance.DisplayUnits = DisplayUnitType.DUT_HOUR_SQUARE_FOOT_FAHRENHEIT_PER_BRITISH_THERMAL_UNIT;
            units.SetFormatOptions(UnitType.UT_HVAC_ThermalResistance, foUTHVACThermalResistance);
            //UTHVACThermalMass
            FormatOptions foUTHVACThermalMass = units.GetFormatOptions(UnitType.UT_HVAC_ThermalMass);
            foUTHVACThermalMass.Accuracy = 0.0001;
            foUTHVACThermalMass.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNIT_PER_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_HVAC_ThermalMass, foUTHVACThermalMass);
            //UTHVACThermalConductivity
            FormatOptions foUTHVACThermalConductivity = units.GetFormatOptions(UnitType.UT_HVAC_ThermalConductivity);
            foUTHVACThermalConductivity.Accuracy = 0.0001;
            foUTHVACThermalConductivity.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_FOOT_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_HVAC_ThermalConductivity, foUTHVACThermalConductivity);
            //UTHVACSpecificHeat
            FormatOptions foUTHVACSpecificHeat = units.GetFormatOptions(UnitType.UT_HVAC_SpecificHeat);
            foUTHVACSpecificHeat.Accuracy = 0.0001;
            foUTHVACSpecificHeat.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_POUND_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_HVAC_SpecificHeat, foUTHVACSpecificHeat);
            //UTHVACSpecificHeatOfVaporization
            FormatOptions foUTHVACSpecificHeatOfVaporization = units.GetFormatOptions(UnitType.UT_HVAC_SpecificHeatOfVaporization);
            foUTHVACSpecificHeatOfVaporization.Accuracy = 0.0001;
            foUTHVACSpecificHeatOfVaporization.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_POUND;
            units.SetFormatOptions(UnitType.UT_HVAC_SpecificHeatOfVaporization, foUTHVACSpecificHeatOfVaporization);
            //UTHVACPermeability
            FormatOptions foUTHVACPermeability = units.GetFormatOptions(UnitType.UT_HVAC_Permeability);
            foUTHVACPermeability.Accuracy = 0.0001;
            foUTHVACPermeability.DisplayUnits = DisplayUnitType.DUT_GRAINS_PER_HOUR_SQUARE_FOOT_INCH_MERCURY;
            units.SetFormatOptions(UnitType.UT_HVAC_Permeability, foUTHVACPermeability);
            //UTElectricalResistivity
            FormatOptions foUTElectricalResistivity = units.GetFormatOptions(UnitType.UT_Electrical_Resistivity);
            foUTElectricalResistivity.Accuracy = 0.0001;
            foUTElectricalResistivity.DisplayUnits = DisplayUnitType.DUT_OHM_METERS;
            units.SetFormatOptions(UnitType.UT_Electrical_Resistivity, foUTElectricalResistivity);
            //UTHVACAirflowDensity
            FormatOptions foUTHVACAirflowDensity = units.GetFormatOptions(UnitType.UT_HVAC_Airflow_Density);
            foUTHVACAirflowDensity.Accuracy = 0.01;
            foUTHVACAirflowDensity.DisplayUnits = DisplayUnitType.DUT_CUBIC_FEET_PER_MINUTE_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Airflow_Density, foUTHVACAirflowDensity);
            //UTSlope
            FormatOptions foUTSlope = units.GetFormatOptions(UnitType.UT_Slope);
            foUTSlope.Accuracy = 0.01;
            foUTSlope.DisplayUnits = DisplayUnitType.DUT_SLOPE_DEGREES;
            units.SetFormatOptions(UnitType.UT_Slope, foUTSlope);
            //UTHVACCoolingLoad
            FormatOptions foUTHVACCoolingLoad = units.GetFormatOptions(UnitType.UT_HVAC_Cooling_Load);
            foUTHVACCoolingLoad.Accuracy = 0.1;
            foUTHVACCoolingLoad.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR;
            units.SetFormatOptions(UnitType.UT_HVAC_Cooling_Load, foUTHVACCoolingLoad);
            //UTHVACHeatingLoad
            FormatOptions foUTHVACHeatingLoad = units.GetFormatOptions(UnitType.UT_HVAC_Heating_Load);
            foUTHVACHeatingLoad.Accuracy = 0.1;
            foUTHVACHeatingLoad.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR;
            units.SetFormatOptions(UnitType.UT_HVAC_Heating_Load, foUTHVACHeatingLoad);
            //UTHVACCoolingLoadDividedByArea
            FormatOptions foUTHVACCoolingLoadDividedByArea = units.GetFormatOptions(UnitType.UT_HVAC_Cooling_Load_Divided_By_Area);
            foUTHVACCoolingLoadDividedByArea.Accuracy = 0.01;
            foUTHVACCoolingLoadDividedByArea.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Cooling_Load_Divided_By_Area, foUTHVACCoolingLoadDividedByArea);
            //UTHVACHeatingLoadDividedByArea
            FormatOptions foUTHVACHeatingLoadDividedByArea = units.GetFormatOptions(UnitType.UT_HVAC_Heating_Load_Divided_By_Area);
            foUTHVACHeatingLoadDividedByArea.Accuracy = 0.01;
            foUTHVACHeatingLoadDividedByArea.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Heating_Load_Divided_By_Area, foUTHVACHeatingLoadDividedByArea);
            //UTHVACCoolingLoadDividedByVolume
            FormatOptions foUTHVACCoolingLoadDividedByVolume = units.GetFormatOptions(UnitType.UT_HVAC_Cooling_Load_Divided_By_Volume);
            foUTHVACCoolingLoadDividedByVolume.Accuracy = 0.01;
            foUTHVACCoolingLoadDividedByVolume.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Cooling_Load_Divided_By_Volume, foUTHVACCoolingLoadDividedByVolume);
            //UTHVACHeatingLoadDividedByVolume
            FormatOptions foUTHVACHeatingLoadDividedByVolume = units.GetFormatOptions(UnitType.UT_HVAC_Heating_Load_Divided_By_Volume);
            foUTHVACHeatingLoadDividedByVolume.Accuracy = 0.01;
            foUTHVACHeatingLoadDividedByVolume.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Heating_Load_Divided_By_Volume, foUTHVACHeatingLoadDividedByVolume);
            //UTHVACAirflowDividedByVolume
            FormatOptions foUTHVACAirflowDividedByVolume = units.GetFormatOptions(UnitType.UT_HVAC_Airflow_Divided_By_Volume);
            foUTHVACAirflowDividedByVolume.Accuracy = 0.01;
            foUTHVACAirflowDividedByVolume.DisplayUnits = DisplayUnitType.DUT_CUBIC_FEET_PER_MINUTE_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Airflow_Divided_By_Volume, foUTHVACAirflowDividedByVolume);
            //UTHVACAirflowDividedByCoolingLoad
            FormatOptions foUTHVACAirflowDividedByCoolingLoad = units.GetFormatOptions(UnitType.UT_HVAC_Airflow_Divided_By_Cooling_Load);
            foUTHVACAirflowDividedByCoolingLoad.Accuracy = 0.01;
            foUTHVACAirflowDividedByCoolingLoad.DisplayUnits = DisplayUnitType.DUT_CUBIC_FEET_PER_MINUTE_TON_OF_REFRIGERATION;
            units.SetFormatOptions(UnitType.UT_HVAC_Airflow_Divided_By_Cooling_Load, foUTHVACAirflowDividedByCoolingLoad);
            //UTHVACAreaDividedByCoolingLoad
            FormatOptions foUTHVACAreaDividedByCoolingLoad = units.GetFormatOptions(UnitType.UT_HVAC_Area_Divided_By_Cooling_Load);
            foUTHVACAreaDividedByCoolingLoad.Accuracy = 0.01;
            foUTHVACAreaDividedByCoolingLoad.DisplayUnits = DisplayUnitType.DUT_SQUARE_FEET_PER_TON_OF_REFRIGERATION;
            units.SetFormatOptions(UnitType.UT_HVAC_Area_Divided_By_Cooling_Load, foUTHVACAreaDividedByCoolingLoad);
            //UTHVACAreaDividedByHeatingLoad
            FormatOptions foUTHVACAreaDividedByHeatingLoad = units.GetFormatOptions(UnitType.UT_HVAC_Area_Divided_By_Heating_Load);
            foUTHVACAreaDividedByHeatingLoad.Accuracy = 0.0001;
            foUTHVACAreaDividedByHeatingLoad.DisplayUnits = DisplayUnitType.DUT_SQUARE_FEET_PER_THOUSAND_BRITISH_THERMAL_UNITS_PER_HOUR;
            units.SetFormatOptions(UnitType.UT_HVAC_Area_Divided_By_Heating_Load, foUTHVACAreaDividedByHeatingLoad);
            //UTWireSize
            FormatOptions foUTWireSize = units.GetFormatOptions(UnitType.UT_WireSize);
            foUTWireSize.Accuracy = 1E-06;
            foUTWireSize.DisplayUnits = DisplayUnitType.DUT_DECIMAL_INCHES;
            units.SetFormatOptions(UnitType.UT_WireSize, foUTWireSize);
            //UTHVACSlope
            FormatOptions foUTHVACSlope = units.GetFormatOptions(UnitType.UT_HVAC_Slope);
            foUTHVACSlope.Accuracy = 0.03125;
            foUTHVACSlope.DisplayUnits = DisplayUnitType.DUT_RISE_OVER_INCHES;
            units.SetFormatOptions(UnitType.UT_HVAC_Slope, foUTHVACSlope);
            //UTPipingSlope
            FormatOptions foUTPipingSlope = units.GetFormatOptions(UnitType.UT_Piping_Slope);
            foUTPipingSlope.Accuracy = 0.03125;
            foUTPipingSlope.DisplayUnits = DisplayUnitType.DUT_RISE_OVER_INCHES;
            units.SetFormatOptions(UnitType.UT_Piping_Slope, foUTPipingSlope);
            //UTCurrency
            FormatOptions foUTCurrency = units.GetFormatOptions(UnitType.UT_Currency);
            foUTCurrency.Accuracy = 0.01;
            foUTCurrency.DisplayUnits = DisplayUnitType.DUT_CURRENCY;
            units.SetFormatOptions(UnitType.UT_Currency, foUTCurrency);
            //UTMassDensity
            FormatOptions foUTMassDensity = units.GetFormatOptions(UnitType.UT_MassDensity);
            foUTMassDensity.Accuracy = 0.01;
            foUTMassDensity.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS_PER_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_MassDensity, foUTMassDensity);
            //UTHVACFactor
            FormatOptions foUTHVACFactor = units.GetFormatOptions(UnitType.UT_HVAC_Factor);
            foUTHVACFactor.Accuracy = 0.01;
            foUTHVACFactor.DisplayUnits = DisplayUnitType.DUT_PERCENTAGE;
            units.SetFormatOptions(UnitType.UT_HVAC_Factor, foUTHVACFactor);
            //UTElectricalTemperature
            FormatOptions foUTElectricalTemperature = units.GetFormatOptions(UnitType.UT_Electrical_Temperature);
            foUTElectricalTemperature.Accuracy = 1;
            foUTElectricalTemperature.DisplayUnits = DisplayUnitType.DUT_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_Electrical_Temperature, foUTElectricalTemperature);
            //UTElectricalCableTraySize
            FormatOptions foUTElectricalCableTraySize = units.GetFormatOptions(UnitType.UT_Electrical_CableTraySize);
            foUTElectricalCableTraySize.Accuracy = 0.25;
            foUTElectricalCableTraySize.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Electrical_CableTraySize, foUTElectricalCableTraySize);
            //UTElectricalConduitSize
            FormatOptions foUTElectricalConduitSize = units.GetFormatOptions(UnitType.UT_Electrical_ConduitSize);
            foUTElectricalConduitSize.Accuracy = 0.125;
            foUTElectricalConduitSize.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Electrical_ConduitSize, foUTElectricalConduitSize);
            //UTElectricalDemandFactor
            FormatOptions foUTElectricalDemandFactor = units.GetFormatOptions(UnitType.UT_Electrical_Demand_Factor);
            foUTElectricalDemandFactor.Accuracy = 0.01;
            foUTElectricalDemandFactor.DisplayUnits = DisplayUnitType.DUT_PERCENTAGE;
            units.SetFormatOptions(UnitType.UT_Electrical_Demand_Factor, foUTElectricalDemandFactor);
            //UTHVACDuctInsulationThickness
            FormatOptions foUTHVACDuctInsulationThickness = units.GetFormatOptions(UnitType.UT_HVAC_DuctInsulationThickness);
            foUTHVACDuctInsulationThickness.Accuracy = 1;
            foUTHVACDuctInsulationThickness.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_HVAC_DuctInsulationThickness, foUTHVACDuctInsulationThickness);
            //UTHVACDuctLiningThickness
            FormatOptions foUTHVACDuctLiningThickness = units.GetFormatOptions(UnitType.UT_HVAC_DuctLiningThickness);
            foUTHVACDuctLiningThickness.Accuracy = 1;
            foUTHVACDuctLiningThickness.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_HVAC_DuctLiningThickness, foUTHVACDuctLiningThickness);
            //UTPipeInsulationThickness
            FormatOptions foUTPipeInsulationThickness = units.GetFormatOptions(UnitType.UT_PipeInsulationThickness);
            foUTPipeInsulationThickness.Accuracy = 1;
            foUTPipeInsulationThickness.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_PipeInsulationThickness, foUTPipeInsulationThickness);
            //UTForce
            FormatOptions foUTForce = units.GetFormatOptions(UnitType.UT_Force);
            foUTForce.Accuracy = 0.01;
            foUTForce.DisplayUnits = DisplayUnitType.DUT_KIPS;
            units.SetFormatOptions(UnitType.UT_Force, foUTForce);
            //UTLinearForce
            FormatOptions foUTLinearForce = units.GetFormatOptions(UnitType.UT_LinearForce);
            foUTLinearForce.Accuracy = 0.001;
            foUTLinearForce.DisplayUnits = DisplayUnitType.DUT_KIPS_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_LinearForce, foUTLinearForce);
            //UTAreaForce
            FormatOptions foUTAreaForce = units.GetFormatOptions(UnitType.UT_AreaForce);
            foUTAreaForce.Accuracy = 0.0001;
            foUTAreaForce.DisplayUnits = DisplayUnitType.DUT_KIPS_PER_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_AreaForce, foUTAreaForce);
            //UTMoment
            FormatOptions foUTMoment = units.GetFormatOptions(UnitType.UT_Moment);
            foUTMoment.Accuracy = 0.01;
            foUTMoment.DisplayUnits = DisplayUnitType.DUT_KIP_FEET;
            units.SetFormatOptions(UnitType.UT_Moment, foUTMoment);
            //UTLinearMoment
            FormatOptions foUTLinearMoment = units.GetFormatOptions(UnitType.UT_LinearMoment);
            foUTLinearMoment.Accuracy = 0.01;
            foUTLinearMoment.DisplayUnits = DisplayUnitType.DUT_KIP_FEET_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_LinearMoment, foUTLinearMoment);
            //UTStress
            FormatOptions foUTStress = units.GetFormatOptions(UnitType.UT_Stress);
            foUTStress.Accuracy = 0.01;
            foUTStress.DisplayUnits = DisplayUnitType.DUT_KIPS_PER_SQUARE_INCH;
            units.SetFormatOptions(UnitType.UT_Stress, foUTStress);
            //UTUnitWeight
            FormatOptions foUTUnitWeight = units.GetFormatOptions(UnitType.UT_UnitWeight);
            foUTUnitWeight.Accuracy = 0.01;
            foUTUnitWeight.DisplayUnits = DisplayUnitType.DUT_POUNDS_FORCE_PER_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_UnitWeight, foUTUnitWeight);
            //UTWeight
            FormatOptions foUTWeight = units.GetFormatOptions(UnitType.UT_Weight);
            foUTWeight.Accuracy = 0.01;
            foUTWeight.DisplayUnits = DisplayUnitType.DUT_POUNDS_FORCE;
            units.SetFormatOptions(UnitType.UT_Weight, foUTWeight);
            //UTMass
            FormatOptions foUTMass = units.GetFormatOptions(UnitType.UT_Mass);
            foUTMass.Accuracy = 0.01;
            foUTMass.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS;
            units.SetFormatOptions(UnitType.UT_Mass, foUTMass);
            //UTMassPerUnitArea
            FormatOptions foUTMassPerUnitArea = units.GetFormatOptions(UnitType.UT_MassPerUnitArea);
            foUTMassPerUnitArea.Accuracy = 0.01;
            foUTMassPerUnitArea.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS_PER_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_MassPerUnitArea, foUTMassPerUnitArea);
            //UTThermalExpansion
            FormatOptions foUTThermalExpansion = units.GetFormatOptions(UnitType.UT_ThermalExpansion);
            foUTThermalExpansion.Accuracy = 1E-05;
            foUTThermalExpansion.DisplayUnits = DisplayUnitType.DUT_INV_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_ThermalExpansion, foUTThermalExpansion);
            //UTForcePerLength
            FormatOptions foUTForcePerLength = units.GetFormatOptions(UnitType.UT_ForcePerLength);
            foUTForcePerLength.Accuracy = 0.1;
            foUTForcePerLength.DisplayUnits = DisplayUnitType.DUT_KIPS_PER_INCH;
            units.SetFormatOptions(UnitType.UT_ForcePerLength, foUTForcePerLength);
            //UTLinearForcePerLength
            FormatOptions foUTLinearForcePerLength = units.GetFormatOptions(UnitType.UT_LinearForcePerLength);
            foUTLinearForcePerLength.Accuracy = 0.1;
            foUTLinearForcePerLength.DisplayUnits = DisplayUnitType.DUT_KIPS_PER_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_LinearForcePerLength, foUTLinearForcePerLength);
            //UTAreaForcePerLength
            FormatOptions foUTAreaForcePerLength = units.GetFormatOptions(UnitType.UT_AreaForcePerLength);
            foUTAreaForcePerLength.Accuracy = 0.1;
            foUTAreaForcePerLength.DisplayUnits = DisplayUnitType.DUT_KIPS_PER_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_AreaForcePerLength, foUTAreaForcePerLength);
            //UTForceLengthPerAngle
            FormatOptions foUTForceLengthPerAngle = units.GetFormatOptions(UnitType.UT_ForceLengthPerAngle);
            foUTForceLengthPerAngle.Accuracy = 0.1;
            foUTForceLengthPerAngle.DisplayUnits = DisplayUnitType.DUT_KIP_FEET_PER_DEGREE;
            units.SetFormatOptions(UnitType.UT_ForceLengthPerAngle, foUTForceLengthPerAngle);
            //UTLinearForceLengthPerAngle
            FormatOptions foUTLinearForceLengthPerAngle = units.GetFormatOptions(UnitType.UT_LinearForceLengthPerAngle);
            foUTLinearForceLengthPerAngle.Accuracy = 0.1;
            foUTLinearForceLengthPerAngle.DisplayUnits = DisplayUnitType.DUT_KIP_FEET_PER_DEGREE_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_LinearForceLengthPerAngle, foUTLinearForceLengthPerAngle);
            //UTDisplacementDeflection
            FormatOptions foUTDisplacementDeflection = units.GetFormatOptions(UnitType.UT_Displacement_Deflection);
            foUTDisplacementDeflection.Accuracy = 0.01;
            foUTDisplacementDeflection.DisplayUnits = DisplayUnitType.DUT_DECIMAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Displacement_Deflection, foUTDisplacementDeflection);
            //UTRotation
            FormatOptions foUTRotation = units.GetFormatOptions(UnitType.UT_Rotation);
            foUTRotation.Accuracy = 0.001;
            foUTRotation.DisplayUnits = DisplayUnitType.DUT_DECIMAL_DEGREES;
            units.SetFormatOptions(UnitType.UT_Rotation, foUTRotation);
            //UTPeriod
            FormatOptions foUTPeriod = units.GetFormatOptions(UnitType.UT_Period);
            foUTPeriod.Accuracy = 0.1;
            foUTPeriod.DisplayUnits = DisplayUnitType.DUT_SECONDS;
            units.SetFormatOptions(UnitType.UT_Period, foUTPeriod);
            //UTStructuralFrequency
            FormatOptions foUTStructuralFrequency = units.GetFormatOptions(UnitType.UT_Structural_Frequency);
            foUTStructuralFrequency.Accuracy = 0.1;
            foUTStructuralFrequency.DisplayUnits = DisplayUnitType.DUT_HERTZ;
            units.SetFormatOptions(UnitType.UT_Structural_Frequency, foUTStructuralFrequency);
            //UTPulsation
            FormatOptions foUTPulsation = units.GetFormatOptions(UnitType.UT_Pulsation);
            foUTPulsation.Accuracy = 0.1;
            foUTPulsation.DisplayUnits = DisplayUnitType.DUT_RADIANS_PER_SECOND;
            units.SetFormatOptions(UnitType.UT_Pulsation, foUTPulsation);
            //UTStructuralVelocity
            FormatOptions foUTStructuralVelocity = units.GetFormatOptions(UnitType.UT_Structural_Velocity);
            foUTStructuralVelocity.Accuracy = 0.1;
            foUTStructuralVelocity.DisplayUnits = DisplayUnitType.DUT_FEET_PER_SECOND;
            units.SetFormatOptions(UnitType.UT_Structural_Velocity, foUTStructuralVelocity);
            //UTAcceleration
            FormatOptions foUTAcceleration = units.GetFormatOptions(UnitType.UT_Acceleration);
            foUTAcceleration.Accuracy = 0.1;
            foUTAcceleration.DisplayUnits = DisplayUnitType.DUT_FEET_PER_SECOND_SQUARED;
            units.SetFormatOptions(UnitType.UT_Acceleration, foUTAcceleration);
            //UTEnergy
            FormatOptions foUTEnergy = units.GetFormatOptions(UnitType.UT_Energy);
            foUTEnergy.Accuracy = 0.1;
            foUTEnergy.DisplayUnits = DisplayUnitType.DUT_POUND_FORCE_FEET;
            units.SetFormatOptions(UnitType.UT_Energy, foUTEnergy);
            //UTReinforcementVolume
            FormatOptions foUTReinforcementVolume = units.GetFormatOptions(UnitType.UT_Reinforcement_Volume);
            foUTReinforcementVolume.Accuracy = 0.01;
            foUTReinforcementVolume.DisplayUnits = DisplayUnitType.DUT_CUBIC_INCHES;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Volume, foUTReinforcementVolume);
            //UTReinforcementLength
            FormatOptions foUTReinforcementLength = units.GetFormatOptions(UnitType.UT_Reinforcement_Length);
            foUTReinforcementLength.Accuracy = 0.00260416666666667;
            foUTReinforcementLength.DisplayUnits = DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Length, foUTReinforcementLength);
            //UTReinforcementArea
            FormatOptions foUTReinforcementArea = units.GetFormatOptions(UnitType.UT_Reinforcement_Area);
            foUTReinforcementArea.Accuracy = 0.01;
            foUTReinforcementArea.DisplayUnits = DisplayUnitType.DUT_SQUARE_INCHES;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Area, foUTReinforcementArea);
            //UTReinforcementAreaperUnitLength
            FormatOptions foUTReinforcementAreaperUnitLength = units.GetFormatOptions(UnitType.UT_Reinforcement_Area_per_Unit_Length);
            foUTReinforcementAreaperUnitLength.Accuracy = 0.01;
            foUTReinforcementAreaperUnitLength.DisplayUnits = DisplayUnitType.DUT_SQUARE_INCHES_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Area_per_Unit_Length, foUTReinforcementAreaperUnitLength);
            //UTReinforcementSpacing
            FormatOptions foUTReinforcementSpacing = units.GetFormatOptions(UnitType.UT_Reinforcement_Spacing);
            foUTReinforcementSpacing.Accuracy = 0.00260416666666667;
            foUTReinforcementSpacing.DisplayUnits = DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Spacing, foUTReinforcementSpacing);
            //UTReinforcementCover
            FormatOptions foUTReinforcementCover = units.GetFormatOptions(UnitType.UT_Reinforcement_Cover);
            foUTReinforcementCover.Accuracy = 0.00260416666666667;
            foUTReinforcementCover.DisplayUnits = DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Cover, foUTReinforcementCover);
            //UTBarDiameter
            FormatOptions foUTBarDiameter = units.GetFormatOptions(UnitType.UT_Bar_Diameter);
            foUTBarDiameter.Accuracy = 0.00260416666666667;
            foUTBarDiameter.DisplayUnits = DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Bar_Diameter, foUTBarDiameter);
            //UTCrackWidth
            FormatOptions foUTCrackWidth = units.GetFormatOptions(UnitType.UT_Crack_Width);
            foUTCrackWidth.Accuracy = 0.01;
            foUTCrackWidth.DisplayUnits = DisplayUnitType.DUT_DECIMAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Crack_Width, foUTCrackWidth);
            //UTSectionDimension
            FormatOptions foUTSectionDimension = units.GetFormatOptions(UnitType.UT_Section_Dimension);
            foUTSectionDimension.Accuracy = 0.00520833333333333;
            foUTSectionDimension.DisplayUnits = DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Section_Dimension, foUTSectionDimension);
            //UTSectionProperty
            FormatOptions foUTSectionProperty = units.GetFormatOptions(UnitType.UT_Section_Property);
            foUTSectionProperty.Accuracy = 0.001;
            foUTSectionProperty.DisplayUnits = DisplayUnitType.DUT_DECIMAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Section_Property, foUTSectionProperty);
            //UTSectionArea
            FormatOptions foUTSectionArea = units.GetFormatOptions(UnitType.UT_Section_Area);
            foUTSectionArea.Accuracy = 0.01;
            foUTSectionArea.DisplayUnits = DisplayUnitType.DUT_SQUARE_INCHES;
            units.SetFormatOptions(UnitType.UT_Section_Area, foUTSectionArea);
            //UTSectionModulus
            FormatOptions foUTSectionModulus = units.GetFormatOptions(UnitType.UT_Section_Modulus);
            foUTSectionModulus.Accuracy = 0.01;
            foUTSectionModulus.DisplayUnits = DisplayUnitType.DUT_CUBIC_INCHES;
            units.SetFormatOptions(UnitType.UT_Section_Modulus, foUTSectionModulus);
            //UTMomentofInertia
            FormatOptions foUTMomentofInertia = units.GetFormatOptions(UnitType.UT_Moment_of_Inertia);
            foUTMomentofInertia.Accuracy = 0.01;
            foUTMomentofInertia.DisplayUnits = DisplayUnitType.DUT_INCHES_TO_THE_FOURTH_POWER;
            units.SetFormatOptions(UnitType.UT_Moment_of_Inertia, foUTMomentofInertia);
            //UTWarpingConstant
            FormatOptions foUTWarpingConstant = units.GetFormatOptions(UnitType.UT_Warping_Constant);
            foUTWarpingConstant.Accuracy = 0.01;
            foUTWarpingConstant.DisplayUnits = DisplayUnitType.DUT_INCHES_TO_THE_SIXTH_POWER;
            units.SetFormatOptions(UnitType.UT_Warping_Constant, foUTWarpingConstant);
            //UTMassperUnitLength
            FormatOptions foUTMassperUnitLength = units.GetFormatOptions(UnitType.UT_Mass_per_Unit_Length);
            foUTMassperUnitLength.Accuracy = 0.01;
            foUTMassperUnitLength.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_Mass_per_Unit_Length, foUTMassperUnitLength);
            //UTWeightperUnitLength
            FormatOptions foUTWeightperUnitLength = units.GetFormatOptions(UnitType.UT_Weight_per_Unit_Length);
            foUTWeightperUnitLength.Accuracy = 0.01;
            foUTWeightperUnitLength.DisplayUnits = DisplayUnitType.DUT_POUNDS_FORCE_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_Weight_per_Unit_Length, foUTWeightperUnitLength);
            //UTSurfaceArea
            FormatOptions foUTSurfaceArea = units.GetFormatOptions(UnitType.UT_Surface_Area);
            foUTSurfaceArea.Accuracy = 0.01;
            foUTSurfaceArea.DisplayUnits = DisplayUnitType.DUT_SQUARE_FEET_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_Surface_Area, foUTSurfaceArea);
            //UTPipeDimension
            FormatOptions foUTPipeDimension = units.GetFormatOptions(UnitType.UT_Pipe_Dimension);
            foUTPipeDimension.Accuracy = 0.001;
            foUTPipeDimension.DisplayUnits = DisplayUnitType.DUT_DECIMAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Pipe_Dimension, foUTPipeDimension);
            //UTPipeMass
            FormatOptions foUTPipeMass = units.GetFormatOptions(UnitType.UT_PipeMass);
            foUTPipeMass.Accuracy = 0.01;
            foUTPipeMass.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS;
            units.SetFormatOptions(UnitType.UT_PipeMass, foUTPipeMass);
            //UTPipeMassPerUnitLength
            FormatOptions foUTPipeMassPerUnitLength = units.GetFormatOptions(UnitType.UT_PipeMassPerUnitLength);
            foUTPipeMassPerUnitLength.Accuracy = 0.01;
            foUTPipeMassPerUnitLength.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_PipeMassPerUnitLength, foUTPipeMassPerUnitLength);





            using (Transaction t = new Transaction(doc, "Convert to Imperial"))
            {
                t.Start();

                doc.SetUnits(units);

                t.Commit();
            }

            return Result.Succeeded;

        }
    }
}
