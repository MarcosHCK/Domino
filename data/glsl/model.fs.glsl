/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
#version 330 core

struct DirLight
{
  vec3 ambient;
  vec3 diffuse;
  vec3 specular;
  vec3 direction;
};

struct PointLight
{
  vec3 ambient;
  vec3 diffuse;
  vec3 specular;
  vec3 position;

  float constant;
  float linear;
  float quadratic;
};

struct SpotLight
{
  vec3 ambient;
  vec3 diffuse;
  vec3 specular; 
  vec3 position;
  vec3 direction;

  float cutOff;
  float outerCutOff;
  
  float constant;
  float linear;
  float quadratic;
};

/* Inputs */
in vec3 Position;
in vec3 Normal;
in vec3 TexCoords;
in vec3 Tangent;
in vec3 Bitangent;

/* Output */
out vec4 FragColor;

/* vertex uniforms */
uniform vec3 aViewPosition;

/* Uniforms */
uniform sampler2DArray aDiffuse;
uniform sampler2DArray aSpecular;
uniform sampler2DArray aNormals;
uniform sampler2DArray aHeight;
uniform DirLight aAmbientLight;
uniform float aShininess;

/*
 * Functions
 */

vec3 CalcDirLight (DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize (- light.direction);

    /* diffuse shading */
    float diff = max (dot (normal, lightDir), 0.0);

    /* specular shading */
    vec3 reflectDir = reflect (-lightDir, normal);
    float spec = pow (max (dot (viewDir, reflectDir), 0.0), aShininess);

    /* combine results */
    vec3 ambient = light.ambient * vec3 (texture (aDiffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3 (texture (aDiffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3 (texture (aSpecular, TexCoords));
    return (ambient + diffuse + specular);
}

vec3 CalcPointLight (PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize (light.position - fragPos);

    /* diffuse shading */
    float diff = max (dot (normal, lightDir), 0.0);

    /* specular shading */
    vec3 reflectDir = reflect (- lightDir, normal);
    float spec = pow (max (dot (viewDir, reflectDir), 0.0), aShininess);

    /* attenuation */
    float distance = length (light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));   
 
    /* combine results */
    vec3 ambient = light.ambient * vec3 (texture (aDiffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3 (texture (aDiffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3 (texture (aSpecular, TexCoords));

    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}

vec3 CalcSpotLight (SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize (light.position - fragPos);

    /* diffuse shading */
    float diff = max (dot (normal, lightDir), 0.0);

    /* specular shading */
    vec3 reflectDir = reflect (- lightDir, normal);
    float spec = pow (max (dot (viewDir, reflectDir), 0.0), aShininess);

    /* attenuation */
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    /* spotlight intensity */
    float theta = dot (lightDir, normalize (- light.direction)); 
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp ((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

    /* combine results */
    vec3 ambient = light.ambient * vec3 (texture (aDiffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3 (texture (aDiffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3 (texture (aSpecular, TexCoords));

    ambient *= attenuation * intensity;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular);
}

/*
 * Main
 */

void main()
{
  vec3 norm = normalize (Normal);
  vec3 viewDir = normalize (aViewPosition - Position);
  vec3 result = vec3 (0, 0, 0);

  /* ambient light */
  result += vec3 (texture (aDiffuse, TexCoords));

  /* emit color */
  FragColor = vec4 (result, 1.0);
}
